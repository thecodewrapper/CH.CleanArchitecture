namespace CH.CleanArchitecture.Presentation.Framework.Interfaces
{
    public interface ICRUDElementHelper
    {
        string GetCRUDIconHTML(CRUDElementTypeEnum type);
        string GetCRUDButtonHtml(CRUDElementTypeEnum type);
    }
}
