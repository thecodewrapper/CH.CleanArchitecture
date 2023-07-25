using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CH.CleanArchitecture.Infrastructure.EntityTypeConfigurations
{
    internal class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItemEntity>
    {
        public void Configure(EntityTypeBuilder<OrderItemEntity> builder) {
            builder.ToTable("OrderItems", ApplicationDbContext.DOMAIN_SCHEMA);
            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.ProductName).IsRequired();
            builder.Property(oi => oi.ProductPrice).IsRequired();
        }
    }
}
