using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CH.CleanArchitecture.Infrastructure.EntityTypeConfigurations
{
    internal class AddressEntityConfiguration : IEntityTypeConfiguration<AddressEntity>
    {
        private readonly string _tableName;
        private readonly string _schema;

        public AddressEntityConfiguration(string tableName, string schema) {
            _tableName = tableName;
            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<AddressEntity> builder) {
            builder.ToTable(_tableName, _schema);
            builder.Property(a => a.Id).HasDefaultValueSql("newsequentialid()");
        }
    }
}
