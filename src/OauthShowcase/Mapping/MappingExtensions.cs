using Mapster;
using OauthShowcase.Contracts;
using OauthShowcase.Domain;

namespace OauthShowcase.Mapping;

public static class MappingExtensions
{
    public static void ConfigureDomainToResponseMapping(this WebApplication app)
    {
        TypeAdapterConfig<User, UserinfoResponse>
            .NewConfig()
            .Map(
                dest => dest.GitlabUsername,
                src => src.ExternalData == null ? null : src.ExternalData.ExternalUserName
            )
            .Map(
                dest => dest.Avatar,
                src => src.Avatar == null ? null : Convert.ToBase64String(src.Avatar)
            )
            .IgnoreNonMapped(false);
    }
}