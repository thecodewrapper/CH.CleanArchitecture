using System;
using CH.Data.Abstractions;

namespace CH.EventStore.EntityFramework.Entities
{
    public class AggregateSnapshotEntity : DataEntityBase<int>
    {
        public string AggregateId { get; set; }
        public string AggregateName { get; set; }
        public int LastAggregateVersion { get; set; }
        public Guid LastEventId { get; set; }
        public string Data { get; set; }
    }
}
