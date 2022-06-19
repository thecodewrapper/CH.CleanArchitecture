using System.Collections.Generic;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Presentation.EventStoreWeb.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        private const string USER = "EventStoreManager";
        public string UserId => USER;

        public string Username => USER;

        public string Name => USER;

        public string Culture => "en-GB";

        public string UiCulture => "en-GB";

        public IEnumerable<RoleEnum> Roles => new List<RoleEnum>() { RoleEnum.SuperAdmin };

        public ThemeEnum Theme => ThemeEnum.Dark;
    }
}
