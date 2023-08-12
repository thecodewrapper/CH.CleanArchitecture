using CH.CleanArchitecture.Common;

namespace CH.CleanArchitecture.Core.Application.DTOs
{
    public abstract class SendNotificationBaseDTO
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
    }
}
