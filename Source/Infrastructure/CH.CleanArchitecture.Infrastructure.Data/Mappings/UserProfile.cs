using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.ReadModels;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Core.Domain.Entities.UserAggregate;
using CH.CleanArchitecture.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CH.CleanArchitecture.Infrastructure.Data.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<ApplicationUserRole, RolesEnum>()
                .ConvertUsing(r => r.Role.Name.ToEnum<RolesEnum>());

            CreateMap<User, ApplicationUser>()
                .ForMember(target => target.Roles, opt => opt.Ignore())
                .ForPath(target => target.PhoneNumber, source => source.MapFrom(m => m.PrimaryPhoneNumber));

            CreateMap<ApplicationUser, User>()
                .ForMember(target => target.PrimaryPhoneNumber, source => source.MapFrom(m => m.PhoneNumber));

            CreateMap<ApplicationUser, UserReadModel>();

            CreateMap<IdentityError, ResultError>()
                .ForMember(target => target.Error, opt => opt.MapFrom(source => source.Description));
        }
    }
}
