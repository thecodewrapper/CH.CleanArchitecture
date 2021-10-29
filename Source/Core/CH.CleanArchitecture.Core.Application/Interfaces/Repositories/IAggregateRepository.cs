using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Core.Application
{
    /// <summary>
    /// Generic marker interface for repository for aggregate root entities
    /// </summary>
    /// <typeparam name="T">The aggregate root type</typeparam>
    /// <typeparam name="TId">The aggregate root id type</typeparam>
    public interface IAggregateRepository<T, TId> : IEntityRepository<T, TId> where T : class, IAggregateRoot<TId>
    {
    }
}