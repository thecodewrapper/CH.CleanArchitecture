namespace CH.CleanArchitecture.Core.Application
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}