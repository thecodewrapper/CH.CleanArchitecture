using System.Collections.Generic;

namespace CH.CleanArchitecture.Core.Application.DTOs
{
    public class SendNotificationDTO : SendNotificationBaseDTO
    {
        public List<string> Recipients { get; set; }
    }
}
