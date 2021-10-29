using CH.CleanArchitecture.Core.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CH.CleanArchitecture.Presentation.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly INotificationService NotificationService;

        public BaseController(INotificationService notificationService) {
            NotificationService = notificationService;
        }
    }
}
