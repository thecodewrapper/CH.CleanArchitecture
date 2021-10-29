using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.DTOs;

namespace CH.CleanArchitecture.Core.Application
{
    public interface IUserAuthenticationService
    {
        Task<Result<string>> Login(LoginRequestDTO loginRequest);
        Task<Result> Logout();
    }
}
