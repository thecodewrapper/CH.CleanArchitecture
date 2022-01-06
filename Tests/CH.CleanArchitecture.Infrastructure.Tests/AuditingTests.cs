using System.Linq;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.CleanArchitecture.Tests;
using Xunit;

namespace CH.CleanArchitecture.Infrastructure.Tests
{
    public class AuditingTests : TestBase
    {
        [Fact]
        public void Auditing_OnAdd_AuditableEntity_CreatesAuditHistoryRecord()
        {
            OrderEntity order = new OrderEntity();
            ApplicationContext.Orders.Add(order);
            ApplicationContext.SaveChanges();

            var auditHistory = ApplicationContext.AuditHistory.SingleOrDefault();

            Assert.NotNull(auditHistory);
        }
    }
}