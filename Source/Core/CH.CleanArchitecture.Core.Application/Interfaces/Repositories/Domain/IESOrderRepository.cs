using System;
using CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate;

namespace CH.CleanArchitecture.Core.Application.Interfaces.Repositories.Domain
{
    public interface IESOrderRepository : IESRepository<Order, Guid>
    {
    }
}
