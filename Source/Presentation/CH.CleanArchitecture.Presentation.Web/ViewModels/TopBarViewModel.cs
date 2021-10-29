using System.Collections.Generic;
using System.Linq;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Presentation.Web.ViewModels
{
    public class TopBarViewModel
    {
        public string Username { get; set; }
        public IEnumerable<RolesEnum> Roles { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsAdmin => Roles.Any(r => r >= RolesEnum.Admin);
        public bool IsSuperAdmin => Roles.Any(r => r >= RolesEnum.SuperAdmin);
    }
}
