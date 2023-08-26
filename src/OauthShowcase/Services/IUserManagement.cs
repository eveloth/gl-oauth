using OauthShowcase.Domain;

namespace OauthShowcase.Services;

public interface IUserManagement
{
    Task Create(User user, CancellationToken ct = default!);
    Task Create(User user, string password, CancellationToken ct = default!);
    Task<User?> Get(int userId, CancellationToken ct = default!);
    Task<User?> Get(string email, CancellationToken ct = default!);
    Task<List<User>> GetAll(CancellationToken ct = default!);
    Task Delete(int userId, CancellationToken ct = default!);
    Task AddExternalData(int userId, ExternalData externalData, CancellationToken ct = default!);
    Task ChangeAvatar(int userId, IFormFile avatar, CancellationToken ct = default!);
    Task DeleteAvatar(int userId, CancellationToken ct = default!);
}