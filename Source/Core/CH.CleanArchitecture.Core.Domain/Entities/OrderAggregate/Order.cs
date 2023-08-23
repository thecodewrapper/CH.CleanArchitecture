using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using CH.Domain.Abstractions;

namespace CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate
{
    public class Order : AggregateRootBase<Guid>,
        IDomainEventHandler<OrderCreatedEvent>,
        IDomainEventHandler<OrderItemAddedEvent>,
        IDomainEventHandler<OrderItemQuantityUpdatedEvent>,
        IDomainEventHandler<OrderBillingAddressAddedEvent>,
        IDomainEventHandler<OrderShippingAddressAddedEvent>
    {
        private List<OrderItem> _orderItems = new List<OrderItem>();

        public Address BillingAddress { get; private set; }
        public Address ShippingAddress { get; private set; }

        public IReadOnlyCollection<OrderItem> OrderItems { get { return _orderItems.AsReadOnly(); } private set { _orderItems = value.ToList(); } }
        public decimal TotalAmount => _orderItems.Sum(oi => oi.ProductPrice * oi.Quantity);
        public string TrackingNumber { get; private set; }

        public Order(string trackingNumber) {
            RaiseEvent(new OrderCreatedEvent(trackingNumber));
        }

        public void AddOrderItem(string productName, decimal productPrice, int quantity) {
            Guard.Against.NegativeOrZero(quantity, nameof(quantity), "Order item quantity cannot be 0 or negative");
            RaiseEvent(new OrderItemAddedEvent(productName, productPrice, quantity));
        }

        public void SetBillingAddress(Address billingAddress) {
            Guard.Against.Null(billingAddress, nameof(billingAddress));
            RaiseEvent(new OrderBillingAddressAddedEvent(billingAddress));
        }

        public void SetShippingAddress(Address shippingAddress) {
            Guard.Against.Null(shippingAddress, nameof(shippingAddress));
            RaiseEvent(new OrderShippingAddressAddedEvent(shippingAddress));
        }

        public void UpdateOrderItemQuantity(Guid orderItemId, int quantity) {
            Guard.Against.NegativeOrZero(quantity, nameof(quantity), "Order item quantity cannot be 0 or negative");
            RaiseEvent(new OrderItemQuantityUpdatedEvent(orderItemId, quantity));
        }

        void IDomainEventHandler<OrderCreatedEvent>.Apply(OrderCreatedEvent @event) {
            Id = @event.AggregateId;
            TrackingNumber = @event.TrackingNumber;
        }

        void IDomainEventHandler<OrderItemAddedEvent>.Apply(OrderItemAddedEvent @event) {
            _orderItems.Add(new OrderItem(@event.ProductName, @event.ProductPrice, @event.Quantity));
        }

        void IDomainEventHandler<OrderItemQuantityUpdatedEvent>.Apply(OrderItemQuantityUpdatedEvent @event) {
            var orderItem = _orderItems.Find(oi => oi.Id == @event.OrderItemId);
            if (orderItem == null) {
                throw new NullReferenceException($"Order item with id {@event.OrderItemId} not found in order {Id}");
            }
            orderItem.UpdateQuantity(@event.Quantity);
        }

        void IDomainEventHandler<OrderShippingAddressAddedEvent>.Apply(OrderShippingAddressAddedEvent @event) {
            ShippingAddress = @event.ShippingAddress;
        }

        void IDomainEventHandler<OrderBillingAddressAddedEvent>.Apply(OrderBillingAddressAddedEvent @event) {
            BillingAddress = @event.BillingAddress;
        }
    }
}