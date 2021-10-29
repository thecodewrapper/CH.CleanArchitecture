using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CH.CleanArchitecture.Presentation.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class DeveloperController : Controller
    {
        public IActionResult Index() {
            return View();
        }
    }
}
