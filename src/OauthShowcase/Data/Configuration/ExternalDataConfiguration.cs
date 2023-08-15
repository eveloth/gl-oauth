using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OauthShowcase.Data.Configuration;

public class ExternalDataConfiguration : IEntityTypeConfiguration<ExternalData>
{
    public void Configure(EntityTypeBuilder<ExternalData> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .HasOne(x => x.User)
            .WithOne(x => x.ExternalData)
            .HasForeignKey<ExternalData>(x => x.UserId);
    }
}