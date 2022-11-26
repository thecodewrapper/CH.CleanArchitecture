using System;
using System.Threading.Tasks;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.ReadModels.Orders;
using CH.Messaging.Abstractions;

namespace CH.CleanArchitecture.Core.Application.Queries.Orders
{
    public class GetOrderByIdQuery : IRequest<Result<OrderReadModel>>
    {
        public Guid OrderId { get; private set; }

        public GetOrderByIdQuery(Guid orderId) {
            OrderId = orderId;
        }
    }

    public class GetOrderByIdQueryHandler : BaseMessageHandler<GetOrderByIdQuery, Result<OrderReadModel>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(IMapper mapper, IOrderRepository orderRepository) {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public override async Task<Result<OrderReadModel>> HandleAsync(GetOrderByIdQuery query) {
            var result = new Result<OrderReadModel>();
            var order = await _orderRepository.FindAsync(query.OrderId);

            result.Data = _mapper.Map<OrderReadModel>(order);

            return result;
        }
    }
}
