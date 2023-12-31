﻿using Mapster;
using OauthShowcase.Contracts;

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
            .IgnoreNonMapped(false);
    }
}