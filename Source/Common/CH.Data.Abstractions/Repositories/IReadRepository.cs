using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CH.Data.Abstractions
{
    /// <summary>
    /// Generic repository interface for read operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadRepository<T>
    {
        /// <summary>
        /// Checks if the data store contains any entity that satisfies the given predicate
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>True/false on whether an entity exists that satisfies the predicate</returns>
        bool Exists(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Asynchronously checks if the data store contains any entity that satisfies the given predicate
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>True/false on whether an entity exists that satisfies the predicate</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Retrieves all entities from the data store as <see cref="IQueryable{T}"/>
        /// </summary>
        /// <returns>Query</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Retrieves all entities that satisfy the given condition from the data store as <see cref="IQueryable{T}"/> 
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>Query</returns>
        IQueryable<T> GetBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Retrieves the first entity from data store that satisfies the given predicate
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>Entity</returns>
        T GetFirst(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Asynchronously retrieves the first entity from the data store that satisfies the given predicate
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>Entity</returns>
        Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Retrieves a single entity from data store that satisfies the given predicate
        /// </summary>
        /// <param name="predicate">Condition</param>
        T GetSingle(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Asynchronously retrieves a single entity from data store that satisfies the given condition
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>Entity</returns>
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);
    }
}