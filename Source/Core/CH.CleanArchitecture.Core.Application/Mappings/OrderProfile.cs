using AutoMapper;
using CH.CleanArchitecture.Core.Application.ReadModels;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate;

namespace CH.CleanArchitecture.Core.Application.Mappings
{
    internal class OrderProfile : Profile
    {
        public OrderProfile() {
            CreateMap<Order, OrderReadModel>();
            CreateMap<OrderItem, OrderItemReadModel>();
            CreateMap<Address, AddressReadModel>();
        }
    }
}
