using BL.Models.DTOs;
using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "AddOrder")]
        public async Task<int> AddOrder(OrderDto order)
        {
            OrderDtoServices orderDtoServices = new();
            ProductionServices productionServices = new();
            int newOrderId = await orderDtoServices.AddOrderAsync(order);
            productionServices.UpdateProductionStateAsync();
            return newOrderId;
        }

        [HttpGet(Name = "GetOrder")]
        public async Task<OrderDto> GetOrder(int orderId)
        {
            OrderDtoServices orderDtoServices = new();
            return await orderDtoServices.GetOrderDtoAsync(orderId);
        }

        [HttpPut(Name = "UpdateOrder")]
        public async Task UpdateOrder(OrderDto order)
        {
            OrderDtoServices orderDtoServices = new();
            ProductionServices productionServices = new();
            await orderDtoServices.UpdateOrderAsync(order);
            productionServices.UpdateProductionStateAsync();
        }
    }
}
