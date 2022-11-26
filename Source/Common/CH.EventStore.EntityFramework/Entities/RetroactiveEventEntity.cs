using System;

namespace CH.EventStore.EntityFramework.Entities
{
    public class RetroactiveEventEntity : EventEntityBase
    {
        /// <summary>
        /// FK of the branch this event belongs to
        /// </summary>
        public int BranchPointId { get; set; }

        /// <summary>
        /// The sequence of the retroactive event
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Indicates whether to ignore or apply this event
        /// </summary>
        public bool IsEnabled { get; set; }

        public virtual BranchPointEntity BranchPoint { get; set; }
    }
}