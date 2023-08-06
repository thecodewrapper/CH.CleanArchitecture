using System;
using CH.Domain.Abstractions;

namespace CH.CleanArchitecture.Core.Domain
{
    public class OrderBillingAddressAddedEvent : DomainEventBase<Guid>
    {
        public Address BillingAddress { get; set; }

        /// <summary>
        /// Needed for serialization
        /// </summary>
        internal OrderBillingAddressAddedEvent() {
        }

        public OrderBillingAddressAddedEvent(Address billingAddress) {
            BillingAddress = billingAddress;
        }

        public OrderBillingAddressAddedEvent(Guid aggregateId, int aggregateVersion, Address billingAddress)
            : base(aggregateId, aggregateVersion) {
            BillingAddress = billingAddress; ;
        }

        public override IDomainEvent<Guid> WithAggregate(Guid aggregateId, int aggregateVersion) {
            return new OrderBillingAddressAddedEvent(aggregateId, aggregateVersion, BillingAddress);
        }
    }
}
