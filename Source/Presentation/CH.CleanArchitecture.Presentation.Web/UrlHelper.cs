using System;

namespace CH.CleanArchitecture.Presentation.Web
{
    public class UrlHelper
    {
        public static string GetReportPhotoUrl(string reportId, Guid photoId) {
            return $@"reports/{reportId}/photos/{photoId}";
        }

        public static string GetProjectAttachmentUrl(Guid projectId, Guid attachmentId) {
            return $@"projects/{projectId}/attachments/{attachmentId}";
        }

        public static string GetReportExportDownloadUrl(string reportId, Guid exportId) {
            return $@"reports/{reportId}/exports/{exportId}";
        }
    }
}
