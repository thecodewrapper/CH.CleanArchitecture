using System.Collections.Generic;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;

namespace CH.CleanArchitecture.Presentation.Framework.Services
{
    public class DefaultModalService : Interfaces.IModalService
    {
        private readonly Blazored.Modal.Services.IModalService _service;

        #region Constructor

        public DefaultModalService(Blazored.Modal.Services.IModalService service) {
            _service = service;
        }

        #endregion

        #region Public Methods

        public IModalReference ShowModal<T>(string title, Dictionary<string, object> parameters = null) where T : ComponentBase {
            ModalParameters modalParams = new ModalParameters();
            foreach (var param in parameters) {
                modalParams.Add(param.Key, param.Value);
            }
            return _service.Show<T>(title, modalParams);
        }

        public IModalReference ShowModal<T>(string title) where T : ComponentBase {
            return _service.Show<T>(title);
        }

        #endregion
    }
}
