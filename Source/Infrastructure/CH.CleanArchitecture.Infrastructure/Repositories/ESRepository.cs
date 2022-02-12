using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.Exceptions;
using CH.CleanArchitecture.Core.Domain;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Infrastructure
{
    ///<inheritdoc cref="IESRepository{T, TId}"/>
    /// <summary>
    /// Repository implementation for event sourcing entities. See <see cref="IAggregateRoot{TId}"/>
    /// Operates directly on <see cref="IAggregateRoot{TId}"/>
    /// </summary>
    /// <typeparam name="T">The aggregate root type</typeparam>
    /// <typeparam name="TId">The aggregate root's id type</typeparam>
    internal class ESRepository<T, TId> : IESRepository<T, TId> where T : class, IAggregateRoot<TId>
    {
        private readonly ILogger<ESRepository<T, TId>> _logger;
        private readonly IEventStore _eventStore;
        private readonly IEventStoreSnapshotProvider _snapshotService;
        private readonly IRetroactiveEventsService _retroEventsService;
        private const int SNAPSHOT_FREQUENCY = 50; //TODO[CH]: Add this to application configurations

        public ESRepository(ILogger<ESRepository<T, TId>> logger, IEventStore eventStore, IEventStoreSnapshotProvider snapshotProvider, IRetroactiveEventsService retroEventsService) {
            _logger = logger;
            _eventStore = eventStore;
            _snapshotService = snapshotProvider;
            _retroEventsService = retroEventsService;
        }

        public async Task<T> GetByIdAsync(TId id) {
            try {
                T aggregate = CreateEmptyAggregate();
                string aggregateName = typeof(T).Name;
                int fromVersion = 0;

                T snapshotAggregate = await _snapshotService.GetAggregateFromSnapshotAsync<T, TId>(id, aggregateName);
                if (snapshotAggregate != default) {
                    aggregate = snapshotAggregate;
                    fromVersion = snapshotAggregate.Version + 1;
                }

                var eventsForAggregate = await _eventStore.LoadAsync<TId>(id, aggregateName, fromVersion, int.MaxValue);

                //if no events are found, return default
                if (!eventsForAggregate.Any() && snapshotAggregate == default) //if no events or snapshot is found
                    throw new EventStoreAggregateNotFoundException($"Aggregate {aggregateName} with id {id} not found");

                eventsForAggregate = _retroEventsService.ApplyRetroactiveEventsToStream<T, TId>(eventsForAggregate);

                foreach (var @event in eventsForAggregate) {
                    aggregate.ApplyEvent(@event, @event.AggregateVersion);
                }

                if (aggregate.IsDeleted)
                    throw new EventStoreAggregateNotFoundException($"Aggregate {aggregateName} with id {id} not found");

                return aggregate;
            }
            catch (Exception ex) {
                _logger.LogError($"Error occured while retrieving from event source repository. Exception: {ex}");
                throw;
            }
        }

        public async Task SaveAsync(T aggregate) {
            try {
                IAggregateRoot<TId> aggregatePersistence = aggregate;
                string aggregateName = typeof(T).Name;
                var uncommittedEvents = aggregate.GetUncommittedEvents();

                if (!uncommittedEvents.Any()) {
                    return;
                }

                Guid lastEventId = uncommittedEvents.Last().EventId;

                await _eventStore.SaveAsync(aggregateName, aggregate.Version, uncommittedEvents);
                if (ShouldSnapshot(aggregate.Version, uncommittedEvents.Count())) {
                    await _snapshotService.SaveSnapshotAsync<T, TId>(aggregate, lastEventId);
                }
                aggregatePersistence.ClearUncommittedEvents();
            }
            catch (Exception ex) {
                _logger.LogError($"Error occured while saving aggregate. Exception: {ex}");
                throw;
            }
        }

        private T CreateEmptyAggregate() {
            return (T)typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>()).Invoke(Array.Empty<object>());
        }

        private bool ShouldSnapshot(int aggregateVersion, int numberOfeventsToCommit) {
            //Every N events we save a snapshot
            return ((aggregateVersion >= SNAPSHOT_FREQUENCY) &&
                (
                    (numberOfeventsToCommit >= SNAPSHOT_FREQUENCY) ||
                    (aggregateVersion % SNAPSHOT_FREQUENCY < numberOfeventsToCommit) ||
                    (aggregateVersion % SNAPSHOT_FREQUENCY == 0)
                )
            );
        }
    }
}