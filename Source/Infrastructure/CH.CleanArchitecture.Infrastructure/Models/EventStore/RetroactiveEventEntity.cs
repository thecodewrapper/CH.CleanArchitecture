using System;

namespace CH.CleanArchitecture.Infrastructure.Models
{
    public class RetroactiveEventEntity : DataEntityBase<Guid>
    {
        /// <summary>
        /// FK of the branch this event belongs to
        /// </summary>
        public int BranchPointId { get; set; }

        /// <summary>
        /// The payload of the retroactive event
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// The sequence of the retroactive event
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// The assembly type name of the retroactive event
        /// </summary>
        public string AssemblyTypeName { get; set; }

        /// <summary>
        /// Indicates whether to ignore or apply this event
        /// </summary>
        public bool IsEnabled { get; set; }

        public virtual BranchPointEntity BranchPoint { get; set; }
    }
}