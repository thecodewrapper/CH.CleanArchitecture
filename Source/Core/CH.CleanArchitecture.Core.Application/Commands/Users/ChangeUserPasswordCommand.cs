using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.Messaging.Abstractions;

namespace CH.CleanArchitecture.Core.Application.Commands
{
    public record ChangeUserPasswordCommand(string Username, string OldPassword, string Password) : IRequest<Result>, ICommand
    {
    }

    /// <summary>
    /// Create User Command Handler
    /// </summary>
    public class ChangeUserPasswordCommandHandler : BaseMessageHandler<ChangeUserPasswordCommand, Result>
    {
        private readonly IApplicationUserService _applicationUserService;

        public ChangeUserPasswordCommandHandler(IApplicationUserService applicationUserService) {
            _applicationUserService = applicationUserService;
        }

        public override async Task<Result> HandleAsync(ChangeUserPasswordCommand command) {
            return await _applicationUserService.ChangePasswordAsync(command.Username, command.OldPassword, command.Password);
        }
    }
}