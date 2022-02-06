using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Tests.Mocks
{
    internal class MockAuthenticatedUserService : IAuthenticatedUserService
    {
        public string UserId => "TestingUser";

        public string Username => "Mr.Tester";

        public string Name => "Tester Tester";

        public IEnumerable<RoleEnum> Roles { get; set; } = new List<RoleEnum>();

        public string Culture => "en-GB";

        public string UiCulture => "en-GB";

        public ThemeEnum Theme => ThemeEnum.Dark;
    }
}
