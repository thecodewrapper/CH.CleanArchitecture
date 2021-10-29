using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Framework
{
    public interface IToastService
    {
        void ShowSuccess(string message);
        void ShowError(string message);
        void ShowError(RenderFragment message);
        void ShowWarning(string message);
    }
}