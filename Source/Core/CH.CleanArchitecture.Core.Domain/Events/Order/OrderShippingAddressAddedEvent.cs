using System;
using CH.Domain.Abstractions;

namespace CH.CleanArchitecture.Core.Domain
{
    public class OrderShippingAddressAddedEvent : DomainEventBase<Guid>
    {
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// Needed for serialization
        /// </summary>
        internal OrderShippingAddressAddedEvent() {
        }

        public OrderShippingAddressAddedEvent(Address shippingAddress) {
            ShippingAddress = shippingAddress;
        }

        public OrderShippingAddressAddedEvent(Guid aggregateId, int aggregateVersion, Address shippingAddress)
            : base(aggregateId, aggregateVersion) {
            ShippingAddress = shippingAddress;
        }

        public override IDomainEvent<Guid> WithAggregate(Guid aggregateId, int aggregateVersion) {
            return new OrderShippingAddressAddedEvent(aggregateId, aggregateVersion, ShippingAddress);
        }
    }
}
