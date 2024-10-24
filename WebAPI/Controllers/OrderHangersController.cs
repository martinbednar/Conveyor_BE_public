using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderHangersController : ControllerBase
    {
        private readonly ILogger<OrderHangersController> _logger;

        public OrderHangersController(ILogger<OrderHangersController> logger)
        {
            _logger = logger;
        }

        [HttpDelete(Name = "DeleteOrderHangers")]
        public async Task DeleteOrderHangers()
        {
            OrderHangerServices orderHangerServices = new OrderHangerServices();
            LogServices logServices = new LogServices();
            await logServices.DeleteAllLogs();
            await orderHangerServices.DeleteAllOrderHangers();
        }
    }
}
