using System.Collections.Generic;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Core.Application
{
    public interface IAuthenticatedUserService
    {
        string UserId { get; }
        public string Username { get; }
        public string Name { get; }
        public IEnumerable<RolesEnum> Roles { get; }
    }
}