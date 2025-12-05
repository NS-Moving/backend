namespace backend;

public class EmailCredentials
{
    public string[] ToAddresses { get; set; } = [];
    public string FromAddress { get; set; } = "";
    public string FromName { get; set; } = "";
    public string FromPassword { get; set; } = "";
    public string SmtpHost { get; set; } = "";
    public int SmtpPort { get; set; }

}