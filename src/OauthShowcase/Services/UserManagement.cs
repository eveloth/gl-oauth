using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OauthShowcase.Data;
using OauthShowcase.Domain;
using OauthShowcase.Errors;

namespace OauthShowcase.Services;

public class UserManagement : IUserManagement
{
    private readonly ApplicationContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserManagement(ApplicationContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task Create(User user, CancellationToken ct = default!)
    {
        await Create(user, string.Empty, ct);
    }

    public async Task Create(User user, string password, CancellationToken ct = default!)
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
    }

    public async Task<User?> Get(int userId, CancellationToken ct = default!)
    {
        return await _context.Users
            .Include(x => x.ExternalData)
            .SingleOrDefaultAsync(x => x.Id == userId, ct);
    }

    public async Task<User?> Get(string email, CancellationToken ct = default!)
    {
        return await _context.Users
            .Include(x => x.ExternalData)
            .SingleOrDefaultAsync(x => x.Email == email, ct);
    }

    public async Task<List<User>> GetAll(CancellationToken ct = default!)
    {
        return await _context.Users.Include(x => x.ExternalData).ToListAsync(ct);
    }

    public async Task Delete(int userId, CancellationToken ct = default!)
    {
        var existingUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId, ct);

        if (existingUser is null)
        {
            throw new ApiException("User doesn't exist");
        }

        _context.Users.Remove(existingUser);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddExternalData(
        int userId,
        ExternalData externalData,
        CancellationToken ct = default!
    )
    {
        var existingUser = await _context.Users
            .Include(user => user.ExternalData)
            .SingleOrDefaultAsync(x => x.Id == userId, ct);

        if (existingUser is null)
        {
            throw new ApiException("User doesn't exist");
        }

        if (existingUser.ExternalData is null)
        {
            existingUser.ExternalData = externalData;
        }
        else
        {
            existingUser.ExternalData.ExternalUserName = externalData.ExternalUserName;
            existingUser.ExternalData.AccessToken = externalData.AccessToken;
            existingUser.ExternalData.RefreshToken = externalData.RefreshToken;
            existingUser.ExternalData.TokenExpiryDate = externalData.TokenExpiryDate;
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task ChangeAvatar(int userId, IFormFile avatar, CancellationToken ct = default!)
    {
        var existingUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId, ct);

        if (existingUser is null)
        {
            throw new ApiException("User doesn't exist");
        }

        using (var memoryStream = new MemoryStream())
        {
            await avatar.CopyToAsync(memoryStream, ct);
            existingUser.Avatar = memoryStream.ToArray();
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAvatar(int userId, CancellationToken ct = default!)
    {
        var existingUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId, ct);

        if (existingUser is null)
        {
            throw new ApiException("User doesn't exist");
        }

        existingUser.Avatar = null;
        await _context.SaveChangesAsync(ct);
    }
}