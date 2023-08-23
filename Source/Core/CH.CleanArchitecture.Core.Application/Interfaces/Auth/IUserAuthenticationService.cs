using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Core.Domain.Entities.UserAggregate;

namespace CH.CleanArchitecture.Core.Application
{
    public interface IUserAuthenticationService
    {
        Task<Result<LoginResponseDTO>> Login(LoginRequestDTO loginRequest);
        Task<Result<LoginResponseDTO>> LoginWith2fa(Login2FARequest login2faRequest);
        Task<Result<LoginResponseDTO>> LoginWithRecoveryCode(Login2FARequest login2faRequest);
        Task<Result<User>> GetTwoFactorAuthenticationUserAsync();
        Task<Result> Logout();
    }
}
