using System;
using System.Collections.Generic;

namespace CH.CleanArchitecture.Core.Application.ReadModels.Orders
{
    public class OrderReadModel
    {
        public Guid Id { get; set; }
        public List<OrderItemReadModel> OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
        public string TrackingNumber { get; set; }
    }
}
