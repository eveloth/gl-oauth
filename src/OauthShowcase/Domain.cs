using System.Security.Claims;
using AspNet.Security.OAuth.GitLab;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OauthShowcase.Identity;

namespace OauthShowcase;

public sealed record User(string Email, string FirstName, string LastName)
{
    public int Id { get; init; }
    public string PasswordHash { get; set; } = default!;
    public ExternalData? ExternalData { get; set; }

    public ClaimsPrincipal ToPrincipal()
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>()
                {
                    new(Claims.Subject, Id.ToString(), ClaimValueTypes.Integer32),
                    new(Claims.Email, Email),
                    new(
                        Claims.GitlabUser,
                        ExternalData is null ? "false" : "true",
                        ClaimValueTypes.Boolean
                    ),
                },
                CookieAuthenticationDefaults.AuthenticationScheme
            )
        );
    }

    public static User FromPrincipal(ClaimsPrincipal principal)
    {
        var name = principal.Claims
            .Single(x => x.Type == GitLabAuthenticationConstants.Claims.Name)
            .Value.Split(' ');

        return new User(
            principal.Claims.Single(x => x.Type == ClaimTypes.Email).Value,
            name[0],
            name[1]
        );
    }
}

public sealed record ExternalData(
    int UserId,
    string Provider,
    string ExternalUserId,
    string ExternalUserName,
    string AccessToken,
    string RefreshToken,
    DateTime TokenExpiryDate
)
{
    public bool IsTokenExpired => DateTime.UtcNow > TokenExpiryDate;
    public User User { get; set; } = default!;
    public int Id { get; set; }
    public string ExternalUserName { get; set; } = ExternalUserName;
    public string AccessToken { get; set; } = AccessToken;
    public string RefreshToken { get; set; } = RefreshToken;
    public DateTime TokenExpiryDate { get; set; } = TokenExpiryDate;

    public static ExternalData FromAuthenticateResult(int userId, AuthenticateResult result)
    {
        var tokens = result.Properties?.GetTokens().ToList()!;

        var externalUserId = result.Principal!.Claims
            .Single(x => x.Type == ClaimTypes.NameIdentifier)
            .Value;
        var externalUserName = result.Principal.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
        var accessToken = tokens.Single(x => x.Name == "access_token").Value;
        var refreshToken = tokens.Single(x => x.Name == "refresh_token").Value;
        var expiryDate = DateTime
            .Parse(tokens.Single(x => x.Name == "expires_at").Value)
            .ToUniversalTime();

        return new ExternalData(
            userId,
            GitLabAuthenticationDefaults.AuthenticationScheme,
            externalUserId,
            externalUserName,
            accessToken,
            refreshToken,
            expiryDate
        );
    }
}