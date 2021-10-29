using System.Threading.Tasks;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.DTOs;

namespace CH.CleanArchitecture.Core.Application.Commands
{
    public record UpdateUserDetailsCommand(string UserId, string Username, string Email, string Name, string PrimaryPhone, string SecondaryPhone) : IRequest<Result>
    {
    }

    /// <summary>
    /// Update User Details Command Handler
    /// </summary>
    public class UpdateUserDetailsCommandHandler : BaseMessageHandler<UpdateUserDetailsCommand, Result>
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly IMapper _mapper;

        public UpdateUserDetailsCommandHandler(IApplicationUserService applicationUserService, IMapper mapper) {
            _applicationUserService = applicationUserService;
            _mapper = mapper;
        }

        public override async Task<Result> HandleAsync(UpdateUserDetailsCommand command) {
            return await _applicationUserService.UpdateUserDetails(_mapper.Map<UpdateUserDetailsDTO>(command));
        }
    }
}