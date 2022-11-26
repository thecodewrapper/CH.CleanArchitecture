using System.Collections.Generic;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.Commands;
using CH.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CH.CleanArchitecture.Presentation.Web.Controllers
{
    [Route("Test")]
    public class TestController : Controller
    {
        private readonly IServiceBus _serviceBus;

        public TestController(IServiceBus serviceBus) {
            _serviceBus = serviceBus;
        }

        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser() {
            Result result = await _serviceBus.SendAsync(
                new CreateUserCommand("dev", "Developer User", "dev@dev.dev", "12345678!!", new List<string>() { "SuperAdmin", "Admin" }));

            if (result.Succeeded) {
                return RedirectToAction("Index", "Home");
            }
            else {
                return NotFound($"{string.Join(',', result.Errors)}");
            }
        }

        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder() {
            Result result = await _serviceBus.SendAsync(new CreateNewOrderCommand("TRACKING NUMBER HERE"));
            if (result.Succeeded) {
                return RedirectToAction("Index", "Home");
            }
            else {
                return NotFound($"{string.Join(',', result.Errors)}");
            }
        }
    }
}