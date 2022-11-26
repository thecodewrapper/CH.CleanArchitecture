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

        /// <summary>
        /// Address line 1
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Address line 2
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Address City
        /// </summary>
        public string AddressCity { get; set; }

        /// <summary>
        /// Address post code
        /// </summary>
        public string AddressPostcode { get; set; }

        /// <summary>
        /// Address country
        /// </summary>
        public string AddressCountry { get; set; }

        /// <summary>
        /// Navigation property for the order's item
        /// </summary>
        public virtual ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
    }
}
