using System.Threading.Tasks;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Framework.Components
{
    public partial class ModalTemplate
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public BlazoredModalInstance Modal { get; set; }
        [Parameter]
        public RenderFragment Body { get; set; }
        [Parameter]
        public RenderFragment Footer { get; set; }

        public async Task Cancel() {
            await Modal.CancelAsync();
        }
    }
}
