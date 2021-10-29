using System;
using CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate;

namespace CH.CleanArchitecture.Core.Application
{
    public interface IOrderRepository : IEntityRepository<Order, Guid>
    {
    }
}