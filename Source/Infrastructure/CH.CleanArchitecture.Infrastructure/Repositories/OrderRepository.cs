using System;
using System.Threading.Tasks;
using AutoMapper;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate;
using CH.CleanArchitecture.Infrastructure.Models;

namespace CH.CleanArchitecture.Infrastructure
{
    /// <summary>
    /// Implementation of <see cref="IOrderRepository"/> which allows persistence on both EventStore and relational store.
    /// </summary>
    internal class OrderRepository : EFRepository<Order, OrderEntity, Guid>, IOrderRepository
    {
        private readonly IESRepository<Order, Guid> _eventStoreRepository;

        public OrderRepository(IMapper mapper, IEntityRepository<OrderEntity, Guid> persistenceRepo, IESRepository<Order, Guid> esRepository)
            : base(mapper, persistenceRepo) {

            _eventStoreRepository = esRepository;
        }

        public async Task SaveToEventStoreAsync(Order order) {
            await _eventStoreRepository.SaveAsync(order);
        }
    }
}
