using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using CH.CleanArchitecture.Presentation.Framework.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CH.CleanArchitecture.Presentation.Framework.Services
{
    public class AuthorizationStateProvider : IAuthorizationStateProvider
    {
        #region Private Members

        private readonly IAuthorizationService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ConcurrentDictionary<IAuthorizationRequirement, bool> _requirements = new();

        #endregion

        #region Constructor

        public AuthorizationStateProvider(IAuthorizationService authService, IHttpContextAccessor httpContextAccessor) {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Public Properties

        public bool this[IAuthorizationRequirement requirement]
        {
            get
            {
                if (_requirements.TryGetValue(requirement, out var result)) {
                    return result;
                }

                return false;
            }
        }

        #endregion

        #region Public Methods

        public async Task AddRequirement(IAuthorizationRequirement requirement) {
            _requirements.TryAdd(requirement, false);

            await EvaluateRequirement(requirement);
        }

        public async Task<bool> TryAddAndCheckRequirement(IAuthorizationRequirement requirement) {
            if (_requirements.TryAdd(requirement, false)) {
                await EvaluateRequirement(requirement);
            }

            return _requirements[requirement];
        }

        public async Task<bool> Any(IAuthorizationRequirement[] requirements) {
            if (requirements is null) {
                return true;
            }

            foreach (var req in requirements) {
                await TryAddAndCheckRequirement(req);

                if (_requirements.TryGetValue(req, out var result)) {
                    if (result) {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task Refresh() {
            var user = _httpContextAccessor.HttpContext.User;
            foreach (var requirement in _requirements.Keys.ToList()) {
                var result = await _authService.AuthorizeAsync(user, null, requirement);



            }
        }

        #endregion

        #region Private Methods

        private async Task EvaluateRequirement(IAuthorizationRequirement requirement) {
            var user = _httpContextAccessor.HttpContext.User;
            var result = await _authService.AuthorizeAsync(user, null, requirement);

            _requirements[requirement] = result.Succeeded;
        }

        #endregion
    }
}
