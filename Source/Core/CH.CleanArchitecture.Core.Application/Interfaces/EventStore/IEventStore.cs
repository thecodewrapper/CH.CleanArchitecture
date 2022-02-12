using System.Collections.Generic;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Core.Application
{
    /// <summary>
    /// Interface containing load/save methods from an Event Store
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Saves a set of events to the event store, for the specified aggregate. See <see cref="IDomainEvent{TAggregateId}"/>
        /// </summary>
        /// <typeparam name="TAggregateId"></typeparam>
        /// <param name="aggregateName">The aggregate name</param>
        /// <param name="expectedVersion">The expected version of the aggregate after events are persisted</param>
        /// <param name="domainEvents">The domain events to persist to the store</param>
        /// <returns></returns>
        Task SaveAsync<TAggregateId>(string aggregateName, int expectedVersion, IEnumerable<IDomainEvent<TAggregateId>> domainEvents);

        /// <summary>
        /// Saves a single event to the event store, for the specified aggregate. See <see cref="IDomainEvent{TAggregateId}"/>
        /// </summary>
        /// <typeparam name="TAggregateId"></typeparam>
        /// <param name="aggregateName"></param>
        /// <param name="expectedVersion"></param>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        Task SaveAsync<TAggregateId>(string aggregateName, int expectedVersion, IDomainEvent<TAggregateId> domainEvent);

        /// <summary>
        /// Retrieves a readonly set of events from the event store, for the given <paramref name="aggregateRootId"/>. 
        /// </summary>
        /// <typeparam name="TAggregateId"></typeparam>
        /// <param name="aggregateRootId">The id of the aggregate</param>
        /// <param name="aggregateName">The aggregate name</param>
        /// <param name="fromVersion">The from version</param>
        /// <param name="toVersion">The to version</param>
        /// <returns></returns>
        Task<IReadOnlyCollection<IDomainEvent<TAggregateId>>> LoadAsync<TAggregateId>(TAggregateId aggregateRootId, string aggregateName, int fromVersion, int toVersion);
    }
}