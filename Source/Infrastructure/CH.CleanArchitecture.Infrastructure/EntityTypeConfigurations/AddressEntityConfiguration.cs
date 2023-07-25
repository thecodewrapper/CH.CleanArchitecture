using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CH.CleanArchitecture.Infrastructure.EntityTypeConfigurations
{
    internal class AddressEntityConfiguration : IEntityTypeConfiguration<AddressEntity>
    {
        private readonly string _schema;

        public AddressEntityConfiguration(string schema) {
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<AddressEntity> builder) {
            builder.ToTable("Addresses", _schema);
            builder.Property(a => a.Id).HasDefaultValueSql("newsequentialid()");
        }
    }
}
