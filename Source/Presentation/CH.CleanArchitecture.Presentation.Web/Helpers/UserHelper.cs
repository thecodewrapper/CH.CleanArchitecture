using System;
using System.Collections.Generic;
using System.Linq;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CH.CleanArchitecture.Presentation.Web.Helpers
{
    public class UserHelper
    {
        private readonly ILocalizationService _localizer;
        private readonly ILocalizationKeyProvider _localizationKeyProvider;
        private readonly IAuthenticatedUserService _userService;

        public UserHelper(IAuthenticatedUserService userService, ILocalizationService localizer, ILocalizationKeyProvider localizationKeyProvider) {
            _localizer = localizer;
            _localizationKeyProvider = localizationKeyProvider;
            _userService = userService;
        }

        public IEnumerable<SelectListItem> GetAvailableRoles() {
            var currentUserHighestRole = _userService.Roles.Max();

            return Enum.GetValues(typeof(RolesEnum))
                                 .Cast<RolesEnum>()
                                 .Where(e => e <= currentUserHighestRole)
                                 .Select(e => new SelectListItem
                                 {
                                     Value = ((int)e).ToString(),
                                     Text = _localizer[_localizationKeyProvider.GetRoleLocalizationKey(e)]
                                 });

        }
    }
}