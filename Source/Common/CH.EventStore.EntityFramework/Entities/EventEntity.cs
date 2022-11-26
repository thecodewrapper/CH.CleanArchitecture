using System;
using System.Collections.Generic;

namespace CH.EventStore.EntityFramework.Entities
{
    public class EventEntity : EventEntityBase
    {
        /// <summary>
        /// Name of the aggregate. This is basically the class name or entity name
        /// </summary>
        public string AggregateName { get; set; }

        /// <summary>
        /// The Id of the aggregate this event is for.
        /// Using this field, events for the given aggregate can be filtered
        /// </summary>
        public string AggregateId { get; set; }

        /// <summary>
        /// Date this event was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The actual name of the event
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Event version.
        /// Increments each time for given aggregate. Used in optimistic concurrency check.
        /// </summary>
        public int Version { get; set; }

        public virtual ICollection<BranchPointEntity> BranchPoints { get; set; }
    }
}