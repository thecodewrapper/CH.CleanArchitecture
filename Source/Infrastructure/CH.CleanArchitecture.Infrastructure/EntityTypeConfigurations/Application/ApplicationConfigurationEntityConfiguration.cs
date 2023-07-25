using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CH.CleanArchitecture.Infrastructure.EntityTypeConfigurations
{
    internal class ApplicationConfigurationEntityConfiguration : IEntityTypeConfiguration<ApplicationConfigurationEntity>
    {
        public void Configure(EntityTypeBuilder<ApplicationConfigurationEntity> builder) {
            builder.ToTable("ApplicationConfigurations", ApplicationDbContext.CONFIG_SCHEMA);
            builder.Property(e => e.Id).HasMaxLength(256);
            builder.Property(e => e.Value).IsRequired().HasMaxLength(512);
            builder.Property(e => e.Description).HasMaxLength(1024);
        }
    }
}
