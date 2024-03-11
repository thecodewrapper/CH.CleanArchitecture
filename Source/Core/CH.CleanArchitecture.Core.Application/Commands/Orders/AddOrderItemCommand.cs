using System;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;

namespace CH.CleanArchitecture.Core.Application.Commands
{
    public class AddOrderItemCommand : BaseCommand<Result>
    {
        public Guid OrderId { get; private set; }
        public string ProductName { get; private set; }
        public decimal ProductPrice { get; private set; }
        public int Quantity { get; private set; }

        public AddOrderItemCommand(Guid orderId, string productName, decimal productPrice, int quantity)
        {
            OrderId = orderId;
            ProductName = productName;
            ProductPrice = productPrice;
            Quantity = quantity;
        }
    }

    /// <summary>
    /// Add Order Item Command Handler
    /// </summary>
    public class AddOrderItemCommandHandler : BaseCommandHandler<AddOrderItemCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;

        public AddOrderItemCommandHandler(IServiceProvider serviceProvider, IOrderRepository orderRepository) : base(serviceProvider) {
            _orderRepository = orderRepository;
        }

        public override async Task<Result> HandleAsync(AddOrderItemCommand command) {
            var order = await _orderRepository.FindAsync(command.OrderId);
            order.AddOrderItem(command.ProductName, command.ProductPrice, command.Quantity);
            _orderRepository.Update(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync();
            return new Result().Succeed();
        }
    }
}
