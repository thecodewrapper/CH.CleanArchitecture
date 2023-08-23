using System.Collections.Generic;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Framework.Interfaces
{
    /// <summary>
    /// Abstraction for a service to show a UI modal window.
    /// Note: Change the return type according to whatever framework you are using (i.e. MudBlazor, Blazored)
    /// </summary>
    public interface IModalService
    {
        /// <summary>
        /// Shows the modal and returns an <see cref="object"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IModalReference ShowModal<T>(string title, Dictionary<string, object> parameters) where T : ComponentBase;

        /// <summary>
        /// Shows the modal and returns an <see cref="object"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        IModalReference ShowModal<T>(string title) where T : ComponentBase;
    }
}
