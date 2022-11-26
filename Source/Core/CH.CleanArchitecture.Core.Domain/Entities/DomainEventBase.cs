using System;
using System.Collections.Generic;
using CH.Domain.Abstractions;

namespace CH.CleanArchitecture.Core.Domain
{
    public abstract class DomainEventBase<TAggregateId> : IDomainEvent<TAggregateId>, IEquatable<DomainEventBase<TAggregateId>>
    {
        public Guid EventId { get; private set; }

        public TAggregateId AggregateId { get; private set; }

        public int AggregateVersion { get; private set; }

        protected DomainEventBase() {
            EventId = Guid.NewGuid();
        }

        protected DomainEventBase(TAggregateId aggregateId) : this() {
            AggregateId = aggregateId;
        }

        protected DomainEventBase(TAggregateId aggregateId, int aggregateVersion) : this(aggregateId) {
            AggregateVersion = aggregateVersion;
        }

        public override bool Equals(object obj) {
            return base.Equals(obj as DomainEventBase<TAggregateId>);
        }

        public bool Equals(DomainEventBase<TAggregateId> other) {
            return other != null &&
                   EventId.Equals(other.EventId);
        }

        public override int GetHashCode() {
            return 290933282 + EqualityComparer<Guid>.Default.GetHashCode(EventId);
        }

        public abstract IDomainEvent<TAggregateId> WithAggregate(TAggregateId aggregateId, int aggregateVersion);
    }
}