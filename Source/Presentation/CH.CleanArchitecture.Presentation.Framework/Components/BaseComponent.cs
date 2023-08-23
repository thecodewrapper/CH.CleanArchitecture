using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Presentation.Framework.Interfaces;
using CH.CleanArchitecture.Presentation.Framework.Services;
using CH.Messaging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CH.CleanArchitecture.Presentation.Framework.Components
{
    public class BaseComponent : ComponentBase
    {
        #region Dependencies

        [Inject]
        public IAuthorizationService AuthService { get; set; }

        [Inject]
        public IAuthorizationStateProvider AuthState { get; set; }

        [Inject]
        public IServiceBus ServiceBus { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        public IToastService ToastService { get; set; }

        [Inject]
        public IModalService ModalService { get; set; }

        [Inject]
        public LoaderService Loader { get; set; }

        [Inject]
        public ILocalizationService LocalizationService { get; set; }

        [Inject]
        public ILocalizationKeyProvider LocalizationKeyProvider { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        #endregion

        #region Public Properties

        public bool ShouldRenderFlag { get; set; } = true;

        #endregion

        #region Public Properties

        protected override bool ShouldRender() {
            return ShouldRenderFlag;
        }

        #endregion

        #region Public Methods

        public async Task<ClaimsPrincipal> GetCurrentUser() {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User;
        }

        public async Task StateHasChangedAsync() {
            await InvokeAsync(() => StateHasChanged());
        }

        public async Task<bool> AuthorizeAsync(OperationAuthorizationRequirement requirement) {
            var authorizationResult = await AuthService.AuthorizeAsync(await GetCurrentUser(), null, requirement);
            return authorizationResult.Succeeded;
        }

        #endregion

        #region Protected Methods


        protected async Task<TResponse> SendRequestAsync<TResponse>(IRequest<TResponse> request, bool showLoader = true, CancellationToken cancellationToken = default) where TResponse : class {
            if (showLoader)
                Loader.Show();

            var result = await ServiceBus.SendAsync(request, cancellationToken);

            if (showLoader)
                Loader.Hide();

            return result;
        }

        #endregion
    }
}
