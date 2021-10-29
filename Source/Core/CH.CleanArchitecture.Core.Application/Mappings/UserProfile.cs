using AutoMapper;
using CH.CleanArchitecture.Core.Application.Commands;
using CH.CleanArchitecture.Core.Application.DTOs;

namespace CH.CleanArchitecture.Core.Application
{
    internal class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<UpdateUserDetailsCommand, UpdateUserDetailsDTO>();
        }
    }
}