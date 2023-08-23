using System.Security.Claims;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common.Constants;
using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CH.CleanArchitecture.Infrastructure.Factories
{
    internal class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor) {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user) {
            var identity = await base.GenerateClaimsAsync(user);

            identity.AddClaim(new Claim(ApplicationClaimTypes.User.Id, user.Id));
            identity.AddClaim(new Claim(ApplicationClaimTypes.User.Username, user.Name));
            identity.AddClaim(new Claim(ApplicationClaimTypes.User.Culture, user.Culture));
            identity.AddClaim(new Claim(ApplicationClaimTypes.User.UiCulture, user.UICulture));
            identity.AddClaim(new Claim(ApplicationClaimTypes.User.Theme, user.Theme.ToString()));

            if (user.ProfilePictureResourceId.HasValue)
                identity.AddClaim(new Claim(ApplicationClaimTypes.User.ProfilePicture, user.ProfilePictureResourceId.Value.ToString()));

            if (user.MustChangePassword)
                identity.AddClaim(new Claim(ApplicationClaimTypes.User.MustChangePassword, string.Empty));

            var roles = await UserManager.GetRolesAsync(user);
            foreach (var role in roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, role));

            return identity;
        }
    }
}
