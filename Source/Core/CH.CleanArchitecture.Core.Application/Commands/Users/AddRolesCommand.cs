using System.Collections.Generic;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.Messaging.Abstractions;

namespace CH.CleanArchitecture.Core.Application.Commands
{
    public record AddRolesCommand(string Username, List<string> Roles) : IRequest<Result>
    {
    }

    /// <summary>
    /// Add roles to user command handler
    /// </summary>
    public class AddRolesCommandHandler : BaseMessageHandler<AddRolesCommand, Result>
    {
        private readonly IApplicationUserService _applicationUserService;

        public AddRolesCommandHandler(IApplicationUserService applicationUserService) {
            _applicationUserService = applicationUserService;
        }

        public override async Task<Result> HandleAsync(AddRolesCommand command) {
            var request = new RoleAssignmentRequestDTO
            {
                Username = command.Username,
                Roles = command.Roles
            };

            return await _applicationUserService.AddRoles(request);
        }
    }
}