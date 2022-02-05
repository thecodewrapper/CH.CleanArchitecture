using System;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate;

namespace CH.CleanArchitecture.Core.Application
{
    public interface IOrderRepository : IAggregateRepository<Order, Guid>
    {
        Task SaveToEventStoreAsync(Order order);
    }
}