using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.ReadModels.Orders;
using CH.Messaging.Abstractions;

namespace CH.CleanArchitecture.Core.Application.Queries.Orders
{
    public class GetAllOrdersQuery : IRequest<Result<List<OrderReadModel>>>
    {
    }

    public class GetAllOrdersQueryHandler : BaseMessageHandler<GetAllOrdersQuery, Result<List<OrderReadModel>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetAllOrdersQueryHandler(IMapper mapper, IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public override async Task<Result<List<OrderReadModel>>> HandleAsync(GetAllOrdersQuery query)
        {
            var result = new Result<List<OrderReadModel>>();
            var orders = _orderRepository.GetAll();

            result.Data = _mapper.Map<List<OrderReadModel>>(orders);

            return result;
        }
    }
}
