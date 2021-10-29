using System;
using System.Linq;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Infrastructure.Data.Models;
using CH.CleanArchitecture.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CH.CleanArchitecture.Infrastructure.Tests
{
    public class EventStoreTests : TestBase
    {
        private readonly IEventStore _eventStore;

        public EventStoreTests() {
            _eventStore = ServiceProvider.GetService<IEventStore>();
        }

        [Fact]
        public async Task EventStore_SaveAsync_SavesEventToStore() {
            string aggregateName = "some aggregate name";
            int aggregateVersion = 1;
            await _eventStore.SaveAsync(aggregateName, aggregateVersion, new OrderItemAddedEvent("some product name", 100, 1));

            var eventsFromStore = EventStoreContext.Events.ToList();

            Assert.NotEmpty(eventsFromStore);
        }

        [Fact]
        public async Task EventStore_LoadAsync_AggregateNotFound_ReturnsEmptyDomainEventList() {
            var events = await _eventStore.LoadAsync<Guid>("aggregate root id which does not exist", "Order", 0, 1);

            Assert.Empty(events);
        }

        [Fact]
        public async Task EventStore_LoadAsync_FromVersion_Negative_ThrowsArgumentException() {
            await Assert.ThrowsAsync<ArgumentException>(async () => await _eventStore.LoadAsync<Guid>("", "", -1, 0));
        }

        [Fact]
        public async Task EventStore_LoadAsync_ToVersion_Negative_ThrowsArgumentException() {
            await Assert.ThrowsAsync<ArgumentException>(async () => await _eventStore.LoadAsync<Guid>("", "", 0, -1));
        }

        [Fact]
        public async Task EventStore_LoadAsync_ToVersionSmallerThanFromVersion_ThrowsArgumentException() {
            await Assert.ThrowsAsync<ArgumentException>(async () => await _eventStore.LoadAsync<Guid>("", "", 6, 5));
        }

        [Fact]
        public async Task EventStore_LoadAsync_AggregateVersionNotFound_ReturnsEmptyDomainEventList() {
            string aggregateId = "8fd1f798-073a-4c40-b1da-f462876c8933";
            AddDummyEvent(typeof(OrderCreatedEvent), aggregateId, 0);
            var events = await _eventStore.LoadAsync<Guid>(aggregateId, "Order", 10, 11);

            Assert.Empty(events);
        }

        [Fact]
        public async Task EventStore_LoadAsync_ReturnsDomainEventsForAggregate_NotEmpty() {
            string aggregateId = "8fd1f798-073a-4c40-b1da-f462876c8933";
            int aggergateVersion = 0;
            AddDummyEvent(typeof(OrderCreatedEvent), aggregateId, aggergateVersion);
            var events = await _eventStore.LoadAsync<Guid>(aggregateId, "Order", aggergateVersion, 1);

            Assert.NotEmpty(events);
        }

        [Fact]
        public async Task EventStore_LoadAsync_ReturnsDomainEventsForAggregate_Events_ContainEventType() {
            string aggregateId = "8fd1f798-073a-4c40-b1da-f462876c8933";
            Type eventType = typeof(OrderCreatedEvent);
            AddDummyEvent(eventType, aggregateId, 0);
            var events = await _eventStore.LoadAsync<Guid>(aggregateId, "Order", 0, 1);

            Assert.IsType(eventType, events.Single());
        }

        private void AddDummyEvent(Type type, string aggregateId, int version) {
            Guid eventId = Guid.NewGuid();
            EventStoreContext.Events.Add(
            new EventEntity
            {
                Id = eventId,
                AggregateName = "Order",
                AggregateId = aggregateId,
                CreatedAt = DateTime.UtcNow,
                Name = type.Name,
                AssemblyTypeName = type.AssemblyQualifiedName,
                Data = "{\"TrackingNumber\":\"TRACKING NUMBER HERE\",\"EventId\":\"" + eventId + "\",\"AggregateId\":\"" + aggregateId + "\",\"AggregateVersion\":" + version + "}",
                Version = version
            });
            EventStoreContext.SaveChanges();
        }
    }
}
