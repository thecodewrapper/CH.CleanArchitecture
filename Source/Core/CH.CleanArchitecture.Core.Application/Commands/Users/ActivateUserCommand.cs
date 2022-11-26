using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.Messaging.Abstractions;

namespace CH.CleanArchitecture.Core.Application.Commands
{
    public record ActivateUserCommand(string Username) : IRequest<Result>
    {
    }

    /// <summary>
    /// Activate User Command Handler
    /// </summary>
    public class ActivateUserCommandHandler : BaseMessageHandler<ActivateUserCommand, Result>
    {
        private readonly IApplicationUserService _applicationUserService;

        public ActivateUserCommandHandler(IApplicationUserService applicationUserService) {
            _applicationUserService = applicationUserService;
        }

        public override async Task<Result> HandleAsync(ActivateUserCommand command) {
            return await _applicationUserService.ActivateUser(command.Username);
        }
    }
}