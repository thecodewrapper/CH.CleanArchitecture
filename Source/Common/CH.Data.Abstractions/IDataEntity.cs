using CH.Domain.Abstractions;

namespace CH.Data.Abstractions
{
    /// <summary>
    /// Generic marker interface for a data model.
    /// Used to specifically identify data (persistence-related) models
    /// </summary>
    /// <typeparam name="TId">The type of Id</typeparam>
    public interface IDataEntity<TId> : IEntity<TId>
    {
    }
}
