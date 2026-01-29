using Microsoft.AspNetCore.Http;

namespace backend;

public class ContactForm
{
    public string? Name { get; }
    public string? PhoneNumber { get; }
    public string? Email { get; }
    public DateOnly Date { get; }
    public string? Description { get; }
    public string? AddOns { get; }
    public string? MovingToAddress { get; }
    public string? MovingFromAddress { get; }

    public List<(Stream stream, string fileName)>? files = [];

    public ContactForm(IFormCollection form)
    {
        Name = form["name"];
        PhoneNumber = form["phoneNumber"];
        Email = form["email"];
        Description = form["description"];
        AddOns = form["addOns"];
        MovingToAddress = form["movingTo"];
        MovingFromAddress = form["movingFrom"];

        if (!DateOnly.TryParse(form["date"], out DateOnly parseDate))
            throw new ArgumentException("Invalid date format");
        else
            Date = parseDate;

        if (IsNullOrWhiteSpaceArray([Name, PhoneNumber, Email, MovingToAddress, MovingFromAddress]))
            throw new ArgumentException("Bad request body");

        foreach (IFormFile file in form.Files)
        {
            Stream stream = file.OpenReadStream();
            string fileName = file.FileName;

            files.Add((stream, fileName));
        }
    }

    // returns true if any of the passed string are null or white space
    private static bool IsNullOrWhiteSpaceArray(params string?[] strings)
    {
        return strings.Any(string.IsNullOrWhiteSpace);
    }

    public string GenerateEmailBody()
    {
        return $"Contact from filled:\n{Name}\n{PhoneNumber}\n{Email}\n\nMoving Date: {Date}\nMoving From: {MovingFromAddress}\nMoving To: {MovingToAddress}\n\nAdd Ons: {AddOns}\nDescription: {Description}";
    }

    public string GenerateEmailSubject()
    {
        return $"{Name} - {Date}";
    }
}