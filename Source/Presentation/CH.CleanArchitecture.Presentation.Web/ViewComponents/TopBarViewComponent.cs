using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Presentation.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CH.CleanArchitecture.Presentation.Web.ViewComponents
{
    [ViewComponent(Name = "TopBar")]
    public class TopBarViewComponent : ViewComponent
    {
        private IAuthenticatedUserService _userService;
        public TopBarViewComponent(IAuthenticatedUserService userService) {
            _userService = userService;
        }

        public IViewComponentResult Invoke() {
            var model = new TopBarViewModel();
            model.Username = _userService.Username;
            model.Roles = _userService.Roles;

            return View(model);
        }
    }
}
