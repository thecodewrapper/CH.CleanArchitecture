using System;

namespace CH.CleanArchitecture.Infrastructure.Models
{
    public class EventEntityBase : DataEntityBase<Guid>
    {
        /// <summary>
        /// Full assembly type name for this event
        /// Used for recreation purposes
        /// </summary>
        public string AssemblyTypeName { get; set; }

        /// <summary>
        /// Payload of the event
        /// </summary>
        public string Data { get; set; }
    }
}
