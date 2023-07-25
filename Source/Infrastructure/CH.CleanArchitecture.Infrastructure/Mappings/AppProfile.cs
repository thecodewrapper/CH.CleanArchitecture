using AutoMapper;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Core.Application.Mappings;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Core.Domain.Entities.OrderAggregate;
using CH.CleanArchitecture.Infrastructure.Auditing;
using CH.CleanArchitecture.Infrastructure.Models;

namespace CH.CleanArchitecture.Infrastructure.Mappings
{
    internal class AppProfile : Profile
    {
        public AppProfile() {
            CreateMap<AuditHistory, AuditHistoryDTO>();

            CreateMap<string, PhoneNumber>().ConvertUsing<StringToPhoneNumberConverter>();
            CreateMap<PhoneNumber, string>().ConvertUsing<PhoneNumberToStringConverter>();
            CreateMap<AddressEntity, Address>().ReverseMap();

            CreateMap<OrderItem, OrderItemEntity>().ReverseMap();
            CreateMap<Order, OrderEntity>();

            CreateMap<OrderEntity, Order>();
        }
    }
}
