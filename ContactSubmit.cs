using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace backend;

public class ContactSubmit
{
    private readonly ILogger<ContactSubmit> _logger;

    public ContactSubmit(ILogger<ContactSubmit> logger)
    {
        _logger = logger;
    }

    // Helper function that safely reads the http request and returns it as an Object, throwing an exception if needed
    static async Task<ContactForm> GetContactForm(HttpRequest req)
    {
        // read the request
        var form = await req.ReadFormAsync();
        return new ContactForm(form);
    }

    static void SendEmail(EmailCredentials credentials, ContactForm reqData)
    {
        // Create an email
        string? mailSubject = reqData.GenerateEmailSubject();
        using MailMessage mail = new();
        foreach(string toAddress in credentials.ToAddresses)
        {
            mail.To.Add(new MailAddress(toAddress));
        }
        mail.From = new MailAddress(credentials.FromAddress, credentials.FromName);
        mail.Subject = mailSubject;
        mail.Body = reqData.GenerateEmailBody();

        // Add attachments
        if (reqData.files != null)
        {
            foreach ((Stream stream, string fileName) in reqData.files)
            {
                // Reset stream position
                if (stream.CanSeek)
                    stream.Position = 0;

                Attachment attachment = new(stream, fileName, "application/octet-stream");
                mail.Attachments.Add(attachment);
            }

        }

        // Send the email
        using SmtpClient smtp = new(credentials.SmtpHost, credentials.SmtpPort);
        smtp.Credentials = new NetworkCredential(credentials.FromAddress, credentials.FromPassword);
        smtp.EnableSsl = true;
        smtp.Send(mail);
    }

    [Function("ContactSubmit")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "contact/submit")] HttpRequest req)
    {
        _logger.LogInformation($"EmailNotification: Received request.");

        try
        {
            // read environment variables
            EmailCredentials emailCredentials = new()
            {
                ToAddresses = Utils.GetEnvArray("toAddress"),
                FromAddress = Utils.GetEnv("fromAddress"),
                FromName = Utils.GetEnv("fromName"),
                FromPassword = Utils.GetEnv("fromPassword"),
                SmtpHost = Utils.GetEnv("smtpHost"),
                SmtpPort = int.Parse(Utils.GetEnv("smtpPort"))
            };

            // read the request
            ContactForm contactForm = await GetContactForm(req);

            // send the email
            SendEmail(emailCredentials, contactForm);

            // log and return success
            _logger.LogInformation($"EmailNotification: request completed successfully.");
            return new OkObjectResult($"Sent email.");

        }
        catch (InvalidOperationException e)
        {
            // missing environment variables
            _logger.LogError(e, $"EmailNotification: {e.Message}");
            return new ObjectResult(e.Message)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        catch (JsonException e)
        {
            // bad date in request body
            _logger.LogError(e, "EmailNotification: Bad date format in request body");
            return new ObjectResult("Bad date format in request body")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        catch (ArgumentException e)
        {
            // body or date missing in request
            _logger.LogError(e, $"EmailNotification: {e.Message}");
            return new BadRequestObjectResult(e.Message);
        }
        catch (Exception e)
        {
            // uncaught exceptions
            _logger.LogError(e, "EmailNotification: An unexpected exception occurred.");
            return new ObjectResult($"An unexpected exception occurred: {e.Message}")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}