using System;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.ReadModels;
using CH.Messaging.Abstractions;

namespace CH.CleanArchitecture.Core.Application.Queries
{
    public class GetOrderByIdQuery : IRequest<Result<OrderReadModel>>, IQuery
    {
        public Guid OrderId { get; private set; }

        public GetOrderByIdQuery(Guid orderId) {
            OrderId = orderId;
        }
    }
}
