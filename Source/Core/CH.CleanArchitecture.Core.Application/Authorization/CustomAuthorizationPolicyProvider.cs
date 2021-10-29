using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CH.CleanArchitecture.Core.Application.Authorization
{
    /// <summary>
    /// Custom authorization policy provider.
    /// Falls back to the <see cref="DefaultAuthorizationPolicyProvider"/> for default and fallback policies
    /// </summary>
    public class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) {
            BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => BackupPolicyProvider.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => BackupPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName) => BackupPolicyProvider.GetPolicyAsync(policyName);

        private AuthorizationPolicy BuildAuthorizationPolicyFromRequirements(params IAuthorizationRequirement[] requirements) {
            var policy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
            policy.AddRequirements(requirements);
            return policy.Build();
        }
    }
}
