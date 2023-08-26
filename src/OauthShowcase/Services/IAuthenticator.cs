using OauthShowcase.Domain;

namespace OauthShowcase.Services;

public interface IAuthenticator
{
    Task<bool> Register(User user, CancellationToken ct = default!);
    Task<bool> Register(User user, string password, CancellationToken ct = default!);
    Task<bool> Register(
        User user,
        string password,
        bool confirmEmail,
        CancellationToken ct = default!
    );

    Task<bool> ConfirmRegistration(
        int userId,
        Guid confirmationToken,
        CancellationToken ct = default!
    );
    Task<bool> SignIn(User user, string password, CancellationToken ct = default!);
}