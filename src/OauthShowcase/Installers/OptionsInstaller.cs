using OauthShowcase.Options;

namespace OauthShowcase.Installers;

public static class OptionsInstaller
{
    public static void ConfigureOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AvatarValidationOptions>(
            builder.Configuration.GetSection(AvatarValidationOptions.AvatarValidation)
        );
        builder.Services.Configure<SpaOptions>(builder.Configuration.GetSection(SpaOptions.Spa));
    }
}