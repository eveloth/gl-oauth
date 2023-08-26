using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using OauthShowcase.Data;
using OauthShowcase.Domain;
using OauthShowcase.Mail;
using OauthShowcase.Services;

namespace OauthShowcase.Installers;

public static class ServiceInstaller
{
    public static void InstallServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthenticator, Authenticator>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<IUserManagement, UserManagement>();
        builder.Services.AddScoped<IMailSender, MailSender>();
        builder.Services.AddScoped<IMailing, Mailing>();
        builder.Services.AddScoped<Seeder>();
        builder.Services.AddScoped<IMapper, Mapper>();
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(IAssemblyMarker));
    }
}