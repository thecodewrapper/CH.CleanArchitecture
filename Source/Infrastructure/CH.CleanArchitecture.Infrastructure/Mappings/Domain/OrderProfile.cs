using AutoMapper;
using CH.CleanArchitecture.Core.Application.ReadModels;
using CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate;
using CH.CleanArchitecture.Infrastructure.Models;

namespace CH.CleanArchitecture.Infrastructure.Mappings
{
    internal class OrderProfile : Profile
    {
        public OrderProfile() {
            CreateMap<OrderItem, OrderItemEntity>().ReverseMap();
            CreateMap<Order, OrderEntity>();
            CreateMap<Order, OrderReadModel>();
            CreateMap<OrderItem, OrderItemReadModel>();

            CreateMap<OrderEntity, Order>();
        }
    }
}
