using AspNet.Security.OAuth.GitLab;
using Microsoft.AspNetCore.Authentication.Cookies;
using OauthShowcase.Options;

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
                }
            );
    }
}