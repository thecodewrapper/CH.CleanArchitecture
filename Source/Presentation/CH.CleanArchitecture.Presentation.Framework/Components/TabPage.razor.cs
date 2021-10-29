using System;
using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Framework.Components
{
    public partial class TabPage : ComponentBase
    {
        private bool _visible = false;


        [CascadingParameter]
        public TabGroup Parent { get; set; }
        [Parameter]
        public string Title { get; set; }
        [Parameter]
        public string IconCss { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public string Id { get; set; }

        protected override void OnInitialized() {
            if (Parent == null) {
                throw new ArgumentNullException(nameof(Parent), "A TabPage can only exist within a TabGroup component");
            }

            Parent.AddPage(this);
            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender) {
            base.OnAfterRender(firstRender);
            _visible = Parent.ActivePage == this;

            StateHasChanged();

        }
    }
}
