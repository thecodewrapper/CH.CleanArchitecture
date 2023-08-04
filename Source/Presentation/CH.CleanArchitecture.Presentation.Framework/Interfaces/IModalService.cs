using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Framework.Interfaces
{
    public interface IModalService
    {
        /// <summary>
        /// Shows the modal and returns an <see cref="object"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object ShowModal<T>(string title, Dictionary<string, object> parameters) where T : ComponentBase;

        /// <summary>
        /// Shows the modal and returns an <see cref="object"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"></param>
        /// <returns></returns>
        object ShowModal<T>(string title) where T : ComponentBase;
    }
}
