using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Framework.Components
{
    /// <summary>
    /// Wizard Step component
    /// </summary>
    public partial class WizardStep : ComponentBase
    {
        /// <summary>
        /// The <see cref="Wizard"/> container
        /// </summary>
        [CascadingParameter]
        protected internal Wizard Parent { get; set; }

        /// <summary>
        /// The Child Content of the current <see cref="WizardStep"/>
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The Name of the step
        /// </summary>
        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string IconCssClass { get; set; }

        protected override void OnInitialized() {
            Parent.AddStep(this);
        }
    }
}
