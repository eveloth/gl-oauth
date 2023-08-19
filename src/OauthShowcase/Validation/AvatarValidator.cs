using FluentValidation;
using Microsoft.Extensions.Options;
using OauthShowcase.Contracts;
using OauthShowcase.Options;

namespace OauthShowcase.Validation;

public class AvatarValidator : AbstractValidator<ChangeAvatarRequest>
{
    private readonly AvatarValidationOptions _validationOptions;

    public AvatarValidator(IOptions<AvatarValidationOptions> validationOptions)
    {
        _validationOptions = validationOptions.Value;

        RuleFor(x => x.Avatar).NotNull().Must(SatisfyRequirements);
    }

    private bool SatisfyRequirements(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);

        if (!_validationOptions.PermittedExtensions.Any(extension.Contains))
        {
            return false;
        }

        return file.Length <= _validationOptions.MaxSizeInBytes;
    }
}