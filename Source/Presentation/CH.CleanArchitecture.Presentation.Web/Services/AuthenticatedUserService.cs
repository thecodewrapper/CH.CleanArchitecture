using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Common.Constants;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;
using Microsoft.AspNetCore.Http;

namespace CH.CleanArchitecture.Presentation.Web.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor) {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null;
            Username = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? null;
            Name = httpContextAccessor.HttpContext?.User?.FindFirst(ApplicationClaimTypes.User.FullName)?.Value ?? null;
            Culture = httpContextAccessor.HttpContext?.User?.FindFirst(ApplicationClaimTypes.User.Culture)?.Value ?? null;
            UiCulture = httpContextAccessor.HttpContext?.User?.FindFirst(ApplicationClaimTypes.User.UiCulture)?.Value ?? null;
            Theme = httpContextAccessor.HttpContext?.User?.FindFirst(ApplicationClaimTypes.User.Theme)?.Value.ToEnum<ThemeEnum>() ?? default;
            var profilePictureClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ApplicationClaimTypes.User.ProfilePicture)?.Value;
            if (profilePictureClaim != null) {
                ProfilePicture = profilePictureClaim;
            }

            var roles = httpContextAccessor.HttpContext?.User?.Claims.Where(c => c.Type == ClaimTypes.Role);
            if (roles != null)
                Roles = roles.Select(r => r.Value.ToEnum<RoleEnum>());
        }

        public string UserId { get; }

        public string Username { get; }
        public string Name { get; }
        public string ProfilePicture { get; }
        public string Culture { get; set; }
        public string UiCulture { get; set; }
        public IEnumerable<RoleEnum> Roles { get; }
        public ThemeEnum Theme { get; private set; }

        public bool HasRole(RoleEnum role) {
            return Roles.Contains(role);
        }
    }
}