using BL.Models.DTOs;
using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Orders")]
        public async Task<List<OrderDto>> GetOrders()
        {
            OrderDtoServices OrderDtoServices = new OrderDtoServices();
            return await OrderDtoServices.GetAllOrdersDtoAsync();
        }
    }
}
