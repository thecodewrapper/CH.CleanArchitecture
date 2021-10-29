using Blazored.Modal;
using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Framework.Interfaces
{
    public interface IModalService
    {
        IModalReference ShowModal<T>(string title, ModalParameters parameters = null) where T : IComponent;

        IModalReference ShowModal<T>(string title) where T : IComponent;
    }
}
