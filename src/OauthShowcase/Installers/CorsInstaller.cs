using OauthShowcase.Options;

namespace OauthShowcase.Installers;

public static class CorsInstaller
{
    public static void InstallSubdomainWildcardCorsPolicy(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
            {
                var corsOptions = builder.Configuration
                    .GetSection(CorsOptions.Cors)
                    .Get<CorsOptions>();
                ArgumentNullException.ThrowIfNull(corsOptions);

                policyBuilder
                    .WithOrigins(corsOptions.RootDomain)
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();

                if (corsOptions.IsLocal)
                {
                    policyBuilder.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback);
                }
            });
        });
    }
}