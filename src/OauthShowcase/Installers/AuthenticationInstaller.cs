using System.Security.Claims;
using AspNet.Security.OAuth.GitLab;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using OauthShowcase.Domain;
using OauthShowcase.Options;
using OauthShowcase.Services;

namespace OauthShowcase.Installers;

public static class AuthenticationInstaller
{
    public static void InstallAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<GitLabOauthOptions>(
            builder.Configuration.GetSection(GitLabOauthOptions.GitLabOauth)
        );

        var gitLabOauthOptions =
            builder.Configuration
                .GetSection(GitLabOauthOptions.GitLabOauth)
                .Get<GitLabOauthOptions>()
            ?? throw new NullReferenceException(nameof(GitLabOauthOptions));

        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(
                CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.Cookie.SameSite = SameSiteMode.Lax;

                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                }
            )
            .AddGitLab(
                GitLabAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.UsePkce = true;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.ClientId = gitLabOauthOptions.ClientId;
                    options.ClientSecret = gitLabOauthOptions.ClientSecret;
                    options.CallbackPath = gitLabOauthOptions.CallbackPath;

                    options.SaveTokens = true;

                    options.Events.OnCreatingTicket = async context => await SyncUser(context);
                }
            );
    }

    private static async Task SyncUser(OAuthCreatingTicketContext context)
    {
        var userManagement =
            context.HttpContext.RequestServices.GetRequiredService<IUserManagement>();

        var email = context.Principal!.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
        var existingUser = await userManagement.Get(email, CancellationToken.None);

        if (existingUser is null)
        {
            await userManagement.Create(
                User.FromPrincipal(context.Principal!),
                CancellationToken.None
            );

            existingUser = await userManagement.Get(email, CancellationToken.None);
        }

        var externalData = ExternalData.FromOauthContext(existingUser!.Id, context);
        await userManagement.AddExternalData(existingUser.Id, externalData, CancellationToken.None);

        context.Principal = existingUser.ToPrincipal();
    }
}