using CH.CleanArchitecture.Core.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CH.CleanArchitecture.Presentation.Web.Validators
{
    public class LocalizedDataAnnotationsValidator : ComponentBase
    {
        [Inject]
        private ILocalizationService Localizer { get; set; }

        [CascadingParameter]
        private EditContext CurrentEditContext { get; set; }

        protected override void OnInitialized() {
            CurrentEditContext.AddLocalizedDataAnnotationsValidation(Localizer);
        }
    }
}
