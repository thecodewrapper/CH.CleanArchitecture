using System;
using System.Collections.Generic;
using CH.CleanArchitecture.Common;
using Microsoft.AspNetCore.Identity;

namespace CH.CleanArchitecture.Infrastructure.Models
{
    public class ApplicationUser : IdentityUser
    {
        #region Public Properties

        /// <summary>
        /// The user's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The user's surname
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// The user's secondary phone number
        /// </summary>
        public string SecondaryPhoneNumber { get; set; }

        /// <summary>
        /// The user's profile picture resource id
        /// </summary> 
        public Guid? ProfilePictureResourceId { get; set; }

        /// <summary>
        /// Flag indicating whether user is active
        /// </summary>
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Flag indicating that user must change his password
        /// </summary>
        public bool MustChangePassword { get; set; } = false;

        /// <summary>
        /// The user's selected theme
        /// </summary>
        public ThemeEnum Theme { get; set; }

        /// <summary>
        /// The user's culture
        /// </summary>
        public string Culture { get; set; } = "el";

        /// <summary>
        /// The user's UI culture
        /// </summary>
        public string UICulture { get; set; } = "el";

        /// <summary>
        /// The user's address
        /// </summary>
        public AddressEntity Address { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<ApplicationUserRole> Roles { get; } = new List<ApplicationUserRole>();

        #endregion
    }

    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
    public class ApplicationRole : IdentityRole
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}