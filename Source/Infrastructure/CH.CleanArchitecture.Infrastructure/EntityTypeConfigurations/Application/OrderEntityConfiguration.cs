using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CH.CleanArchitecture.Infrastructure.EntityTypeConfigurations
{
    internal class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder) {
            builder.ToTable("Orders", ApplicationDbContext.DOMAIN_SCHEMA);
            builder.Property(o => o.TotalAmount).IsRequired();
            builder.HasOne(o => o.BillingAddress).WithOne().OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(o => o.ShippingAddress).WithOne().OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(o => o.OrderItems).WithOne(oi => oi.Order).HasForeignKey(o => o.OrderId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
