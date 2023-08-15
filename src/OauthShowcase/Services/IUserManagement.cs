namespace OauthShowcase.Services;

public interface IUserManagement
{
    Task Create(User user, CancellationToken ct);
    Task Create(User user, string password, CancellationToken ct);
    Task<User?> Get(int userId, CancellationToken ct);
    Task<User?> Get(string email, CancellationToken ct);
    Task<List<User>> GetAll(CancellationToken ct);
    Task<User?> Login(string email, string password, CancellationToken ct);
    Task AddExternalData(int userId, ExternalData externalData, CancellationToken ct);
}