using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Common.Constants;
using CH.CleanArchitecture.Common.Extensions;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Presentation.Web.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        private const string FALLBACK_USERNAME = "SystemUser";
        private readonly ILogger<AuthenticatedUserService> _logger;

        public string UserId { get; }
        public string Username { get; }
        public string Name { get; }
        public string ProfilePicture { get; }
        public string Culture { get; set; }
        public string UiCulture { get; set; }
        public IEnumerable<RoleEnum> Roles { get; }
        public ThemeEnum Theme { get; private set; }
        public ClaimsPrincipal User { get; private set; }


        public AuthenticatedUserService(ILogger<AuthenticatedUserService> logger, IHttpContextAccessor httpContextAccessor) {
            _logger = logger;
            try {
                var user = GetUser(httpContextAccessor);
                User = user;
                UserId = user.FindId() ?? null;
                Username = user.FindFullName() ?? null;
                Name = user.FindFullName() ?? null;
                Culture = user.FindCulture() ?? null;
                UiCulture = user.FindUiCulture() ?? null;
                Theme = user.FindTheme()?.ToEnum<ThemeEnum>() ?? default;
                var profilePictureClaim = user.FindProfilePicture();
                if (profilePictureClaim != null) {
                    ProfilePicture = profilePictureClaim;
                }

                var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role);
                if (roles != null)
                    Roles = roles.Select(r => r.Value.ToEnum<RoleEnum>());
            }
            catch (Exception ex) {
                _logger.LogWarning($"Error while instantiating {nameof(AuthenticatedUserService)}", ex);
                Username = FALLBACK_USERNAME;
            }
        }

        public bool HasRole(RoleEnum role) {
            return Roles.Contains(role);
        }

        private ClaimsPrincipal GetUser(IHttpContextAccessor httpContextAccessor) {
            if (httpContextAccessor.HttpContext != null) {
                return httpContextAccessor.HttpContext.User;
            }

            return new ClaimsPrincipal();
        }
    }
}