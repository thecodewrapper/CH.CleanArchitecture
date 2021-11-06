using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CH.CleanArchitecture.Infrastructure.Identity
{
    internal class UserTokensConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder) {
            builder.ToTable("UserTokens");
        }
    }
}
