namespace CH.Data.Abstractions
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}