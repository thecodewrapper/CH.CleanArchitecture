using System.Linq;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.CleanArchitecture.Tests;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CH.CleanArchitecture.Infrastructure.Tests
{
    public class AuditingTests : TestBase
    {
        [Fact]
        public void Auditing_OnAdd_AuditableEntity_CreatesAuditHistoryRecord() {
            OrderEntity order = new OrderEntity();
            order.TotalAmount = 100;
            ApplicationContext.Orders.Add(order);
            ApplicationContext.SaveChanges();
            ApplicationContext.DetachAll();

            var auditHistory = ApplicationContext.AuditHistory.SingleOrDefault(ah => ah.TableName == nameof(OrderEntity) && ah.Kind == EntityState.Added);

            Assert.NotNull(auditHistory);
        }
    }
}