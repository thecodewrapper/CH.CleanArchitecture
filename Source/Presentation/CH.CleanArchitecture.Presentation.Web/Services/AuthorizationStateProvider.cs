using System.Security.Claims;
using System.Threading.Tasks;
using CH.CleanArchitecture.Presentation.Framework.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CH.CleanArchitecture.Presentation.Web.Services
{
    public class AuthorizationStateProvider : IAuthorizationStateProvider
    {
        private readonly IAuthorizationService _authService;

        public AuthorizationStateProvider(IAuthorizationService authService) {
            _authService = authService;
        }

        public async Task<bool> CheckRequirement(ClaimsPrincipal user, IAuthorizationRequirement requirement) {
            var authResult = await _authService.AuthorizeAsync(user, null, requirement);
            return authResult.Succeeded;
        }

        public async Task<bool> CheckRequirement(ClaimsPrincipal user, object resource, IAuthorizationRequirement requirement) {
            var authResult = await _authService.AuthorizeAsync(user, resource, requirement);
            return authResult.Succeeded;
        }
    }
}
