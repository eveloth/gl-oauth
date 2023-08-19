namespace OauthShowcase.Contracts;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
public record ChangeAvatarRequest(IFormFile Avatar);