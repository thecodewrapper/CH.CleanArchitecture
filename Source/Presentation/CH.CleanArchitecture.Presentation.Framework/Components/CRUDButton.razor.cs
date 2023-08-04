using System.Threading.Tasks;
using CH.CleanArchitecture.Infrastructure.Resources;
using CH.CleanArchitecture.Presentation.Framework.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CH.CleanArchitecture.Presentation.Framework.Components
{
    public partial class CRUDButton : BaseComponent
    {
        [Inject] public ICRUDElementHelper CRUDElementHelper { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickAction { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public CRUDElementTypeEnum Type { get; set; }

        [Parameter]
        public string CssClass { get; set; }

        private string _icon;
        private string _btn;

        protected override async Task OnParametersSetAsync() {

            _icon = CRUDElementHelper.GetCRUDIconHTML(Type);
            _btn = CRUDElementHelper.GetCRUDButtonHtml(Type);

            Title ??= GetTitle();

            await base.OnParametersSetAsync();
        }

        private string GetTitle() {
            switch (Type) {
                case CRUDElementTypeEnum.View: return LocalizationService[ResourceKeys.Buttons_Details];
                case CRUDElementTypeEnum.Delete: return LocalizationService[ResourceKeys.Buttons_Delete];
                case CRUDElementTypeEnum.Edit: return LocalizationService[ResourceKeys.Buttons_Edit];
                case CRUDElementTypeEnum.Save: return LocalizationService[ResourceKeys.Buttons_Save];
                case CRUDElementTypeEnum.Cancel: return LocalizationService[ResourceKeys.Buttons_Cancel];
                default: return "";
            }
        }
    }
}
