using FluentEmail.Core;

namespace OauthShowcase.Mail;

public class MailSender : IMailSender
{
    private readonly IFluentEmail _fluentEmail;

    public MailSender(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task<bool> Send<T>(string to, string template, T parameters, CancellationToken ct = default!)
    {
        return (
            await _fluentEmail.To(to).UsingTemplate(template, parameters).SendAsync(ct)
        ).Successful;
    }
}