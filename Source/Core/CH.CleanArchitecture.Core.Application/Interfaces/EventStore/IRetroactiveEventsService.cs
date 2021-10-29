using System.Collections.Generic;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Core.Application
{
    public interface IRetroactiveEventsService
    {
        /// <summary>
        /// Apply retroactive events to the event stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAggregateId"></typeparam>
        /// <param name="eventStream"></param>
        /// <returns>An event stream with the retroactive events injected</returns>
        IReadOnlyCollection<IDomainEvent<TAggregateId>> ApplyRetroactiveEventsToStream<T, TAggregateId>(IReadOnlyCollection<IDomainEvent<TAggregateId>> eventStream) where T : class, IAggregateRoot<TAggregateId>;
    }
}