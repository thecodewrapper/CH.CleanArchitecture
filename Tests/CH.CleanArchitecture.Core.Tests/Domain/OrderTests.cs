using System;
using System.Linq;
using CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate;
using Xunit;

namespace CH.CleanArchitecture.Core.Domain.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_Create_SetsTrackingNumber() {
            string trackingNumber = "1234";
            Order order = new Order(trackingNumber);

            Assert.Equal(trackingNumber, order.TrackingNumber);
        }

        [Fact]
        public void Order_AddOrderItem_OrderItems_NotEmpty() {
            Order order = new Order("");
            order.AddOrderItem("", 1, 1);

            Assert.NotEmpty(order.OrderItems);
        }

        [Fact]
        public void Order_AddOrderItem_AddsOrderItemWithSpecifiedData() {
            string productName = "product name";
            decimal productPrice = 10;
            int quantity = 1;

            Order order = new Order("");
            order.AddOrderItem(productName, productPrice, quantity);
            var orderItem = order.OrderItems.Single();

            Assert.Equal(productName, orderItem.ProductName);
            Assert.Equal(productPrice, orderItem.ProductPrice);
            Assert.Equal(quantity, orderItem.Quantity);
        }

        [Fact]
        public void Order_AddOrderItem_OnZeroQuantity_ThrowsArgumentException() {
            Order order = new Order("");
            Assert.Throws<ArgumentException>(() => order.AddOrderItem("", 1, 0));
        }

        [Fact]
        public void Order_AddOrderItem_OnNegativeQuantity_ThrowsArgumentException() {
            Order order = new Order("");
            Assert.Throws<ArgumentException>(() => order.AddOrderItem("", -1, 0));
        }

        [Fact]
        public void Order_UpdateOrderItemQuantity_Quantity_EqualToSet() {
            int previousQuantity = 1;
            int updatedQuantity = 2;
            Order order = new Order("");
            order.AddOrderItem("productName", 10, previousQuantity);
            Guid orderItemId = order.OrderItems.Single().Id;

            order.UpdateOrderItemQuantity(orderItemId, updatedQuantity);

            Assert.Equal(updatedQuantity, order.OrderItems.Single().Quantity);
        }

        [Fact]
        public void OrderItem_UpdateOrderItemQuantity_OrderItemNotFound_ThrowsNullReferenceException() {
            Order order = new Order("");
            Assert.Throws<NullReferenceException>(() => order.UpdateOrderItemQuantity(Guid.NewGuid(), 1));
        }

        [Fact]
        public void Order_UpdateOrderItemQuantity_QuantityNegative_ThrowsArgumentException() {
            Order order = new Order("");
            order.AddOrderItem("productName", 10, 1);
            Guid orderItemId = order.OrderItems.Single().Id;
            Assert.Throws<ArgumentException>(() => order.UpdateOrderItemQuantity(Guid.NewGuid(), -1));
        }

        [Fact]
        public void Order_UpdateOrderItemQuantity_QuantityZero_ThrowsArgumentException() {
            Order order = new Order("");
            order.AddOrderItem("productName", 10, 1);
            Guid orderItemId = order.OrderItems.Single().Id;
            Assert.Throws<ArgumentException>(() => order.UpdateOrderItemQuantity(Guid.NewGuid(), 0));
        }
    }
}
