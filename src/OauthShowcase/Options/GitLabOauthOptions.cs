namespace OauthShowcase.Options;

public class GitLabOauthOptions
{
    public const string GitLabOauth = "GitLab";

    public string CallbackPath { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
}