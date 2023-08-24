using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using OauthShowcase.Services;

namespace OauthShowcase.Installers;

public static class ServiceInstaller
{
    public static void InstallServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserManagement, UserManagement>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<IMapper, Mapper>();
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(IAssemblyMarker));
    }
}