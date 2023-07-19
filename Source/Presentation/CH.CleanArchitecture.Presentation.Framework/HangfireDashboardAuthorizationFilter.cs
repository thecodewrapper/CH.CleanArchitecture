using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CH.CleanArchitecture.Presentation.Framework
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HangfireDashboardAuthorizationFilter(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor) {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Authorize([NotNull] DashboardContext context) {
            var httpContext = _httpContextAccessor.HttpContext;

            var authorized = _authorizationService.AuthorizeAsync(
                httpContext.User,
                "HangfireDashboardPolicy").ConfigureAwait(false).GetAwaiter().GetResult();

            return authorized.Succeeded;
        }
    }
}
