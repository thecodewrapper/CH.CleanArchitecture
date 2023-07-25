using System;
using System.Collections.Generic;
using CH.Data.Abstractions;

namespace CH.CleanArchitecture.Infrastructure.Models
{
    /// <summary>
    /// Data entity for Order
    /// </summary>
    public class OrderEntity : DataEntityBase<Guid>
    {
        /// <summary>
        /// The order's tracking number
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// The order's total amount
        /// </summary>
        public decimal TotalAmount { get; set; }

        public AddressEntity BillingAddress { get; set; }
        public AddressEntity ShippingAddress { get; set; }

        /// <summary>
        /// Navigation property for the order's item
        /// </summary>
        public virtual ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
    }
}
