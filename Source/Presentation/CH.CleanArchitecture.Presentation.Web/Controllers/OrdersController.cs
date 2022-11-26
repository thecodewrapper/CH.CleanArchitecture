using System.Linq;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.Queries.Orders;
using CH.CleanArchitecture.Presentation.Web.ViewModels.Orders;
using CH.Messaging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CH.CleanArchitecture.Presentation.Web.Controllers
{
    [Route("Orders")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IServiceBus _serviceBus;

        public OrdersController(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }
        /// <summary>
        /// Returns a view of all orders
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            IndexViewModel model = new();
            var orders = await _serviceBus.SendAsync(new GetAllOrdersQuery());

            model.Orders = orders.Data
                .Select(o => new OrderViewModel() { Id = o.Id, TotalAmount = o.TotalAmount, TrackingNumber = o.TrackingNumber })
                .ToList();
            
            return View(model);
        }
    }
}
