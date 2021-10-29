using System.Collections.Generic;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;

namespace CH.CleanArchitecture.Core.Application.Commands
{
    public record UpdateUserRolesCommand(string Username, List<string> Roles) : IRequest<Result>
    {
    }

    /// <summary>
    /// Update user roles command handler
    /// </summary>
    public class UpdateUserRolesCommandHandler : BaseMessageHandler<UpdateUserRolesCommand, Result>
    {
        private readonly IApplicationUserService _applicationUserService;

        public UpdateUserRolesCommandHandler(IApplicationUserService applicationUserService) {
            _applicationUserService = applicationUserService;
        }

        public override async Task<Result> HandleAsync(UpdateUserRolesCommand command) {
            return await _applicationUserService.UpdateRoles(command.Username, command.Roles);
        }
    }
}