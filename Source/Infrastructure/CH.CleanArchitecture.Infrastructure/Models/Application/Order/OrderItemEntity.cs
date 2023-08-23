using System;
using CH.Data.Abstractions;

namespace CH.CleanArchitecture.Infrastructure.Models
{
    public class OrderItemEntity : DataEntityBase<Guid>
    {
        /// <summary>
        /// FK of Order for which this order item belongs to
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// The order item product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// The order item product price
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// The order item product quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Navigation property for this item's Order.
        /// </summary>
        public virtual OrderEntity Order { get; set; }
    }
}
