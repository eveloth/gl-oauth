namespace OauthShowcase.Options;

public class SpaOptions
{
    public const string Spa = "Spa";
    public bool Enabled { get; set; }
    public string MainPageUrl { get; set; } = default!;
}