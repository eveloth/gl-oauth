namespace OauthShowcase.Mail;

public class MailingOptions
{
    public const string Mail = "Mail";
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Sender { get; set; } = default!;
}