namespace OauthShowcase.Options;

public class CorsOptions
{
    public const string Cors = "Cors";
    public string RootDomain { get; set; } = default!;
    public bool IsLocal { get; set; }
}