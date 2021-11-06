using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CH.CleanArchitecture.Infrastructure.Identity
{
    internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder) {
            builder.ToTable("Roles");
        }
    }
}
