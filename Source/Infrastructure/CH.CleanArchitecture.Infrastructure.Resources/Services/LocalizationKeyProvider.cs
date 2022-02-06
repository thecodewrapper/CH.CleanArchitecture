using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;

namespace CH.CleanArchitecture.Infrastructure.Resources
{
    public class LocalizationKeyProvider : ILocalizationKeyProvider
    {
        public string GetRoleLocalizationKey(RoleEnum role) {
            return role switch
            {
                RoleEnum.User => ResourceKeys.Roles_User,
                RoleEnum.Admin => ResourceKeys.Roles_Admin,
                RoleEnum.SuperAdmin => ResourceKeys.Roles_SuperAdmin,
                _ => role.ToString()
            };
        }
    }
}
