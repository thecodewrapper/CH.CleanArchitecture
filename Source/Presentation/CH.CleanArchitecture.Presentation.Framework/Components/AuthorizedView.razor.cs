using System;
using System.Threading.Tasks;
using CH.CleanArchitecture.Presentation.Framework.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Framework.Components
{
    public partial class AuthorizedView : BaseComponent
    {
        [Inject]
        public IAuthorizationStateProvider AuthorizationState { get; set; }

        [Parameter]
        public Func<bool> Expression { get; set; }

        [Parameter]
        public IAuthorizationRequirement Operation { get; set; }

        [Parameter]
        public RenderFragment Success { get; set; }

        [Parameter]
        public RenderFragment Failed { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public bool Visible { get; set; }

        protected override async Task OnParametersSetAsync() {
            var user = await GetCurrentUserAsync();
            Visible = await AuthorizationState.CheckRequirement(user, Operation);

            await base.OnParametersSetAsync();
        }
    }
}
