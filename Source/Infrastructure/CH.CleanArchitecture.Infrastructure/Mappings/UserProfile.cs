using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.ReadModels;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Core.Domain.Entities.UserAggregate;
using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace CH.CleanArchitecture.Infrastructure.Mappings
{
    internal class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<ApplicationUserRole, RoleEnum>()
                .ConvertUsing(r => r.Role.Name.ToEnum<RoleEnum>());

            CreateMap<User, ApplicationUser>()
                .ForMember(target => target.Roles, opt => opt.Ignore());

            CreateMap<ApplicationUser, User>();

            CreateMap<ApplicationUser, UserReadModel>();

            CreateMap<IdentityError, ResultError>()
                .ForMember(target => target.Error, opt => opt.MapFrom(source => source.Description));
        }
    }
}
