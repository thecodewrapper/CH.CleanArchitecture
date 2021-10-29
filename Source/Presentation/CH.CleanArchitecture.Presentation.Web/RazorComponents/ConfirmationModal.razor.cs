using System.Threading.Tasks;
using Blazored.Modal;
using CH.CleanArchitecture.Infrastructure.Resources;
using CH.CleanArchitecture.Presentation.Framework.Components;
using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Web.RazorComponents
{
    public partial class ConfirmationModal : BaseComponent
    {
        [CascadingParameter]
        public BlazoredModalInstance ModalInstance { get; set; }
        [Parameter]
        public string Message { get; set; }
        [Parameter]
        public string YesCaption { get; set; }
        [Parameter]
        public string NoCaption { get; set; }

        protected override async Task OnParametersSetAsync() {
            if (string.IsNullOrEmpty(YesCaption))
                YesCaption = LocalizationService[ResourceKeys.Common_Yes];

            if (string.IsNullOrEmpty(NoCaption))
                NoCaption = LocalizationService[ResourceKeys.Common_No];

            await base.OnParametersSetAsync();
        }

        protected async Task Ok() {
            await ModalInstance.CloseAsync();
        }

        protected async Task Cancel() {
            await ModalInstance.CancelAsync();
        }
    }
}
