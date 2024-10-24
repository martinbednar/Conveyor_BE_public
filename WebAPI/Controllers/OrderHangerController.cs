using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderHangerController : ControllerBase
    {
        private readonly ILogger<OrderHangerController> _logger;

        public OrderHangerController(ILogger<OrderHangerController> logger)
        {
            _logger = logger;
        }

        [HttpDelete(Name = "DeleteOrderHanger")]
        public async Task DeleteOrderHanger(int orderHangerId)
        {
            OrderHangerServices orderHangerServices = new OrderHangerServices();
            OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();
            orderHangerSectionServices.CancelAllNotLeftSectionsForOrderHanger(orderHangerId);
            orderHangerServices.DeleteOrderHanger(orderHangerId);
            await Task.CompletedTask;
        }
    }
}
