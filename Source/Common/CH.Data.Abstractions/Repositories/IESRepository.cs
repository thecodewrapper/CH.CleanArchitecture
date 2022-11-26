using CH.Domain.Abstractions;

namespace CH.Data.Abstractions
{
    /// <summary>
    /// Generic interface for event-sourcing repository for aggregate roots. See <see cref="IAggregateRoot{TAggregateId}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IESRepository<T, TId> where T : IAggregateRoot<TId>
    {
        /// <summary>
        /// Retrieves an entity from event store by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(TId id);

        /// <summary>
        /// Saves the aggregate to event store
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        Task SaveAsync(T aggregate);
    }
}