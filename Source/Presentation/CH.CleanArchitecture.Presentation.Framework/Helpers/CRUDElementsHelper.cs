using CH.CleanArchitecture.Presentation.Framework.Interfaces;

namespace CH.CleanArchitecture.Presentation.Framework
{
    public class CRUDElementsHelper : ICRUDElementHelper
    {
        public string GetCRUDIconHTML(CRUDElementTypeEnum type) {
            switch (type) {
                case CRUDElementTypeEnum.View: return "<i class=\"fas fa-info\"></i>";
                case CRUDElementTypeEnum.Delete: return "<i class=\"fas fa-trash\"></i>";
                case CRUDElementTypeEnum.Edit: return "<i class=\"fas fa-edit\"></i>";
                case CRUDElementTypeEnum.Save: return "<i class=\"fas fa-save\"></i>";
                case CRUDElementTypeEnum.Cancel: return "<i class=\"fas fa-times\"></i>";
                default: return "";
            }
        }

        public string GetCRUDButtonHtml(CRUDElementTypeEnum type) {
            switch (type) {
                case CRUDElementTypeEnum.View: return "btn-info";
                case CRUDElementTypeEnum.Delete: return "btn-danger";
                case CRUDElementTypeEnum.Edit: return "btn-warning";
                case CRUDElementTypeEnum.Save: return "btn-success";
                case CRUDElementTypeEnum.Cancel: return "btn-danger";
                default: return "";
            }
        }
    }
}
