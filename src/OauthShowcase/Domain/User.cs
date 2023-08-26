using System.Security.Claims;
using AspNet.Security.OAuth.GitLab;
using Microsoft.AspNetCore.Authentication.Cookies;
using OauthShowcase.Identity;

namespace OauthShowcase.Domain;

public sealed record User(string Email, string FirstName, string LastName)
{
    public int Id { get; init; }
    public string PasswordHash { get; set; } = default!;
    public byte[]? Avatar { get; set; }
    public bool Confirmed { get; set; }
    public Guid? ConfirmationToken { get; set; }
    public ExternalData? ExternalData { get; set; }

    public ClaimsPrincipal ToPrincipal()
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                new List<Claim>
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