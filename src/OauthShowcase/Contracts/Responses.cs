namespace OauthShowcase.Contracts;

public record UserinfoResponse(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    string? GitlabUsername,
    string? Avatar
);