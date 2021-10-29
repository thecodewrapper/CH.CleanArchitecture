using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Core.Application
{
    /// <summary>
    /// Interface for generic repository for domain entities. See <see cref="IEntity{TId}"/>.
    /// Inherits from <see cref="IReadRepository{T}"/> and <see cref="IWriteRepository{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IEntityRepository<T, TId> : IReadRepository<T>, IWriteRepository<T> where T : class, IEntity<TId>
    {
        /// <summary>
        /// Finds an entity from data store.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Find(TId id);

        /// <summary>
        /// Asynchronously finds an entity from data store.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> FindAsync(TId id);
    }
}