using System;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common.Extensions;
using CH.CleanArchitecture.Infrastructure.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace CH.CleanArchitecture.Infrastructure.Shared.Culture
{
    public class UserProfileRequestCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext) {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (!httpContext.User.Identity.IsAuthenticated) {
                return Task.FromResult(new ProviderCultureResult("en", "en"));
            }

            string userCulture = null;
            string userUICulture = null;

            string cultureClaim = httpContext.User.FindCulture();
            if (!string.IsNullOrWhiteSpace(cultureClaim)) {
                userCulture = cultureClaim;
            }

            string uicultureClaim = httpContext.User.FindUiCulture();
            if (!string.IsNullOrWhiteSpace(uicultureClaim)) {
                userUICulture = uicultureClaim;
            }

            if (userCulture == null && userUICulture == null) {
                // No values specified for either so no match
                return Task.FromResult((ProviderCultureResult)null);
            }

            if (userCulture != null && userUICulture == null) {
                // Value for culture but not for UI culture so default to culture value for both
                userUICulture = userCulture;
            }

            if (userCulture == null && userUICulture != null) {
                // Value for UI culture but not for culture so default to UI culture value for both
                userCulture = userUICulture;
            }

            var requestCulture = new ProviderCultureResult(userCulture, userUICulture);

            return Task.FromResult(requestCulture);
        }
    }
}
