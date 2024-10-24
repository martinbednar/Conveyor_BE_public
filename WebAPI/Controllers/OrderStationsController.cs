using BL.Models.DTOs;
using BL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderStationsController : ControllerBase
    {
        private readonly ILogger<OrderStationsController> _logger;

        public OrderStationsController(ILogger<OrderStationsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetActiveOrderForStation")]
        public async Task<OrderDto?> GetActiveOrderForStation(StationEnum station)
        {
            OrderDtoServices orderDtoServices = new();
            return await orderDtoServices.GetActiveOrderDtoForStationAsync(station);
        }

        [HttpPut(Name = "ValidateOrderStations")]
        public async Task<string> ValidateOrderStations(OrderDto order)
        {
            OrderStationsServices orderStationsServices = new OrderStationsServices();
            return await orderStationsServices.ValidateOrderStationsAsync(order);
        }
    }
}
