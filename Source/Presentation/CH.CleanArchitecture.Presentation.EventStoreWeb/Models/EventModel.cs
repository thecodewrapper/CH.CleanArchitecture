namespace CH.CleanArchitecture.Presentation.EventStoreWeb.Models
{
    public class EventModel : EventModelBase
    {
        /// <summary>
        /// Unique identifier of the event
        /// </summary>
        public Guid Id { get; set; }
        
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

        /// <summary>
        /// The event's branch points, if any
        /// </summary>
        public List<BranchPointModel> BranchPoints { get; set; } = new List<BranchPointModel>();
    }
}