using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
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
    .AddGitLab(options =>
    {
        options.ClientId = gitLabOauthOptions.ClientId;
        options.ClientSecret = gitLabOauthOptions.ClientSecret;
        options.CallbackPath = gitLabOauthOptions.CallbackPath;
    });

builder.Services.AddControllers(options =>
{
    // Apply transformer defined below
    options.Conventions.Add(
        new RouteTokenTransformerConvention(new ToSlugCaseTransformerConvention())
    );
});

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

public partial class ToSlugCaseTransformerConvention : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value is null
            ? null
            : ToSlugCaseTransformerRegex().Replace(value.ToString()!, "$1-$2").ToLower();
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex ToSlugCaseTransformerRegex();
}