using System;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate;

namespace CH.CleanArchitecture.Core.Application.Commands
{
    public class CreateNewOrderCommand : BaseCommand<Result>
    {
        public string TrackingNumber { get; private set; }

        public CreateNewOrderCommand(string trackingNumber)
        {
            TrackingNumber = trackingNumber;
        }
    }

    /// <summary>
    /// Create New Order Command Handler
    /// </summary>
    public class CreateNewOrderCommandHandler : BaseCommandHandler<CreateNewOrderCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateNewOrderCommandHandler(IServiceProvider serviceProvider, IOrderRepository orderRepository) : base(serviceProvider) {
            _orderRepository = orderRepository;
        }

        public override async Task<Result> HandleAsync(CreateNewOrderCommand command) {
            Order order = new Order(command.TrackingNumber);
            order.AddOrderItem("Some product name", 10, 1);
            Address address = new Address("testline1", "testline2", "testcity", "testPostcode", "testcountry");
            order.SetShippingAddress(address);
            order.SetBillingAddress(address);
            await _orderRepository.AddAsync(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync();
            await _orderRepository.SaveToEventStoreAsync(order); //saving also to event store
            return new Result().Succeed();
        }
    }
}
