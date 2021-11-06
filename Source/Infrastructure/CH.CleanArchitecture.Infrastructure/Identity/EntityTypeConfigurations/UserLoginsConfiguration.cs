using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CH.CleanArchitecture.Infrastructure.Identity
{
    internal class UserLoginsConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder) {
            builder.ToTable("UserLogins");
        }
    }
}
