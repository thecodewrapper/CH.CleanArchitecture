using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate;
using CH.CleanArchitecture.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CH.CleanArchitecture.Core.Tests.Application.Repositories
{
    public class OrderRepositoryTests : TestBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventStore _eventStore;

        public OrderRepositoryTests()
        {
            _orderRepository = ServiceProvider.GetService<IOrderRepository>();
            _eventStore = ServiceProvider.GetService<IEventStore>();
        }

        [Fact]
        public async Task SaveToEventStore_NewOrder_OrderCreatedEvent_ExistsInStore()
        {
            var order = new Order("1234");
            await _orderRepository.SaveToEventStoreAsync(order);

            var events = await _eventStore.LoadAsync<Guid>(order.Id.ToString(), nameof(Order), 0, 1);

            Assert.NotEmpty(events);
            Assert.IsType<OrderCreatedEvent>(events.Single());
        }

        [Fact]
        public async Task SaveToEventStore_NewOrder_OrderCreatedEvent_TrackingNumber_SameAsOrder()
        {
            string trackingNumber = "1234";
            var order = new Order(trackingNumber);
            await _orderRepository.SaveToEventStoreAsync(order);

            var events = await _eventStore.LoadAsync<Guid>(order.Id.ToString(), nameof(Order), 0, 1);

            Assert.Equal(trackingNumber, ((OrderCreatedEvent)events.Single()).TrackingNumber);
        }
    }
}
