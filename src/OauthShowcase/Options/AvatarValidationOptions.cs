namespace OauthShowcase.Options;

public class AvatarValidationOptions
{
    public const string AvatarValidation = "FileValidation:Avatar";
    public long MaxSizeInBytes { get; set; }
    public string[] PermittedExtensions { get; set; } = default!;
}