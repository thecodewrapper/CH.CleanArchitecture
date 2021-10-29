using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Tests.Mocks
{
    internal class MockAuthenticatedUserService : IAuthenticatedUserService
    {
        public string UserId => "TestingUser";

        public string Username => "Mr.Tester";

        public string Name => "Tester Tester";

        public IEnumerable<RolesEnum> Roles { get; set; } = new List<RolesEnum>();
    }
}
