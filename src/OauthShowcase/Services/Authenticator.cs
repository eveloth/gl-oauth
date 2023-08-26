using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OauthShowcase.Data;
using OauthShowcase.Domain;
using OauthShowcase.Errors;
using OauthShowcase.Mail;

namespace OauthShowcase.Services;

public class Authenticator : IAuthenticator
{
    private readonly ApplicationContext _context;
    private readonly IMailSender _mailSender;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Authenticator(
        ApplicationContext context,
        IMailSender mailSender,
        IPasswordHasher<User> passwordHasher,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _context = context;
        _mailSender = mailSender;
        _passwordHasher = passwordHasher;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> Register(User user, CancellationToken ct = default!)
    {
        return await Register(user, string.Empty, false, ct);
    }

    public async Task<bool> Register(User user, string password, CancellationToken ct = default)
    {
        return await Register(user, password, true, ct);
    }

    public async Task<bool> Register(
        User user,
        string password,
        bool confirmEmail = false,
        CancellationToken ct = default!
    )
    {
        var existingUser = await _context.Users.SingleOrDefaultAsync(
            x => x.Email == user.Email,
            ct
        );

        if (existingUser is not null)
        {
            throw new ApiException("User already exists");
        }

        var passwordHash = _passwordHasher.HashPassword(user, password);
        user.PasswordHash = passwordHash;

        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);

        if (!confirmEmail)
        {
            return true;
        }

        var registrationTemplate = await _context.EmailTemplates.SingleAsync(
            x => x.Name == MailingDefaults.ConfirmRegistrationTemplateName,
            ct
        );

        user.ConfirmationToken = Guid.NewGuid();

        var wasEmailSent = await _mailSender.Send(
            user.Email,
            registrationTemplate.Template,
            user,
            ct
        );

        if (!wasEmailSent)
        {
            return false;
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ConfirmRegistration(
        int userId,
        Guid confirmationToken,
        CancellationToken ct = default!
    )
    {
        var user = await _context.Users.FindAsync(new object?[] { userId }, cancellationToken: ct);

        if (user is null)
        {
            return false;
        }

        if (user.ConfirmationToken != confirmationToken)
        {
            return false;
        }

        user.Confirmed = true;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SignIn(User user, string password, CancellationToken ct = default!)
    {
        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password
        );

        if (verificationResult is not PasswordVerificationResult.Success)
        {
            return false;
        }

        await _httpContextAccessor.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            user.ToPrincipal()
        );

        return true;
    }
}