using System;

namespace CH.CleanArchitecture.Core.Domain
{
    public class OrderItemAddedEvent : DomainEventBase<Guid>
    {
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal ProductPrice { get; private set; }

        /// <summary>
        /// Needed for serialization
        /// </summary>
        internal OrderItemAddedEvent() {
        }

        public OrderItemAddedEvent(string productName, decimal productPrice, int quantity) {
            ProductName = productName;
            Quantity = quantity;
            ProductPrice = productPrice;
        }

        public OrderItemAddedEvent(Guid aggregateId, int aggregateVersion, string productName, decimal productPrice, int quantity)
            : base(aggregateId, aggregateVersion) {
            ProductName = productName;
            Quantity = quantity;
            ProductPrice = productPrice;
        }

        public override IDomainEvent<Guid> WithAggregate(Guid aggregateId, int aggregateVersion) {
            return new OrderItemAddedEvent(aggregateId, aggregateVersion, ProductName, ProductPrice, Quantity);
        }
    }
}
