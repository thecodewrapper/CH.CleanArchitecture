using System;

namespace CH.CleanArchitecture.Core.Application
{
    /// <summary>
    /// Marker interface for an event
    /// </summary>
    public interface IEvent
    {
        Guid EventId { get; }
    }
}
