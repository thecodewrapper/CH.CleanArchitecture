using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class DefaultAuthenticatedUserService : IAuthenticatedUserService
    {
        public string UserId => string.Empty;
        public string Username => string.Empty;
        public string Name => string.Empty;
        public string Culture => string.Empty;
        public string UiCulture => string.Empty;
        public IEnumerable<RoleEnum> Roles => Enumerable.Empty<RoleEnum>();
        public ThemeEnum Theme => ThemeEnum.Light;
        public ClaimsPrincipal User => new ClaimsPrincipal();
    }
}
