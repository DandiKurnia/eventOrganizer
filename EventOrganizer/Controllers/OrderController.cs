using EventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EventOrganizer.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrder _orderRepository;
        public OrderController(IOrder orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            var userId = Guid.Parse(userIdString);

            var vendor = await _orderRepository.Get(userId);
            return View(vendor);
        }

    }
}
