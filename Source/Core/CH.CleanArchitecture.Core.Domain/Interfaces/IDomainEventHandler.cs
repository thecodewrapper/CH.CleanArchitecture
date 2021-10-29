namespace CH.CleanArchitecture.Core.Domain
{
    internal interface IDomainEventHandler<T> where T : IDomainEvent
    {
        internal void Apply(T @event);
    }
}
