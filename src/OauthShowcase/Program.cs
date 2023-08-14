using Chemodanchik.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using OauthShowcase.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GitLabOauthOptions>(
    builder.Configuration.GetSection(GitLabOauthOptions.GitLabOauth)
);

var gitLabOauthOptions =
    builder.Configuration.GetSection(GitLabOauthOptions.GitLabOauth).Get<GitLabOauthOptions>()
    ?? throw new NullReferenceException(nameof(GitLabOauthOptions));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGitLab(
        GitLabOauthOptions.GitLabOauth,
        options =>
        {
            options.UsePkce = true;
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            options.ClientId = gitLabOauthOptions.ClientId;
            options.ClientSecret = gitLabOauthOptions.ClientSecret;
            options.CallbackPath = gitLabOauthOptions.CallbackPath;
        }
    );

builder.Services.AddControllers(options => options.UseSlugCaseRoutes());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();