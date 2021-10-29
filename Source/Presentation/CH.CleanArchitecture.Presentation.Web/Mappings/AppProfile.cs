using System.Linq;
using AutoMapper;
using CH.CleanArchitecture.Core.Application.Commands;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Core.Application.Mappings;
using CH.CleanArchitecture.Core.Application.ReadModels;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Core.Domain.Entities.UserAggregate;
using CH.CleanArchitecture.Presentation.Web.ViewModels;

namespace CH.CleanArchitecture.Presentation.Web.Mappings
{
    public class AppProfile : Profile
    {
        public AppProfile() {
            #region Commands

            CreateMap<CreateUserViewModel, CreateUserCommand>()
                .ForMember(target => target.Roles, opt => opt.MapFrom(m => m.Roles.Select(r => r.ToString())));
            CreateMap<EditUserViewModel, UpdateUserDetailsCommand>();
            CreateMap<EditUserViewModel, UpdateUserRolesCommand>()
                 .ForMember(target => target.Roles, opt => opt.MapFrom(m => m.Roles.Select(r => r.ToString())));

            #endregion

            #region User

            CreateMap<LoginViewModel, LoginRequestDTO>();
            CreateMap<UserReadModel, EditUserViewModel>();
            CreateMap<UserReadModel, UserDetailsModel>();
            CreateMap<User, UserReadModel>()
               .ForMember(target => target.LocalizedRoles, source => source.MapFrom<LocalizedRolesResolver>());

            #endregion

            CreateMap<string, PhoneNumber>().ConvertUsing<StringToPhoneNumberConverter>();
            CreateMap<PhoneNumber, string>().ConvertUsing<PhoneNumberToStringConverter>();
        }
    }
}
