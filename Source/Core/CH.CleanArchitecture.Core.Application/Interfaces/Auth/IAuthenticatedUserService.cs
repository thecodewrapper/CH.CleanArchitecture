using System.Collections.Generic;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Core.Application
{
    public interface IAuthenticatedUserService
    {
        string UserId { get; }
        public string Username { get; }
        public string Name { get; }
        public string Culture { get; }
        public string UiCulture { get; }
        public IEnumerable<RoleEnum> Roles { get; }
        public ThemeEnum Theme { get; }
    }
}