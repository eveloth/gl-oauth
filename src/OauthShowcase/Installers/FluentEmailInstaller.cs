using FluentEmail.MailKitSmtp;
using MailKit.Security;
using OauthShowcase.Mail;
using OauthShowcase.Options;

namespace OauthShowcase.Installers;

public static class FluentEmailInstaller
{
    public static void InstallFluentEmail(this WebApplicationBuilder builder)
    {
        var mailOptions = builder.Configuration.GetSection(MailingOptions.Mail).Get<MailingOptions>();

        builder.Services
            .AddFluentEmail(mailOptions.Sender)
            .AddLiquidRenderer()
            .AddMailKitSender(
                new SmtpClientOptions
                {
                    Server = mailOptions.Host,
                    Port = mailOptions.Port,
                    User = mailOptions.Username,
                    Password = mailOptions.Password,
                    RequiresAuthentication = true,
                    SocketOptions = SecureSocketOptions.StartTls
                }
            );
    }
}