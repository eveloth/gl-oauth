namespace OauthShowcase.Mail;

public interface IMailSender
{
    Task<bool> Send<T>(string to, string template, T parameters, CancellationToken ct = default!);
}