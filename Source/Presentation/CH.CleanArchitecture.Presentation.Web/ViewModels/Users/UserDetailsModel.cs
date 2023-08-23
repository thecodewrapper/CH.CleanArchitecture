using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Infrastructure.Resources;

namespace CH.CleanArchitecture.Presentation.Web.ViewModels
{
    public class UserDetailsModel
    {
        public Guid Id { get; set; }

        [Display(Name = ResourceKeys.Labels_Username)]
        public string Username { get; set; }

        [Display(Name = ResourceKeys.Labels_Name)]
        public string Name { get; set; }

        [Display(Name = ResourceKeys.Labels_Email)]
        public string Email { get; set; }

        [Display(Name = ResourceKeys.Labels_Phone)]
        public string PhoneNumber { get; set; }

        [Display(Name = ResourceKeys.Labels_Roles)]
        public IEnumerable<RoleEnum> Roles { get; set; }

        [Display(Name = ResourceKeys.Labels_Status)]
        public bool IsActive { get; set; }

        public IReadOnlyCollection<string> LocalizedRoles { get; set; }

    }
}
