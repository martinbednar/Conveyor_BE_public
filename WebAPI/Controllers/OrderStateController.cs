using BL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderStateController : ControllerBase
    {
        private readonly ILogger<OrderStateController> _logger;

        public OrderStateController(ILogger<OrderStateController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "OrderState")]
        public async Task OrderState([FromQuery] int orderId, [FromQuery] string orderState)
        {
            OStateEnum newOrderState;
            switch (orderState)
            {
                // PRIVATE - NOT PUBLISHED
            }
            if (newOrderState != OStateEnum.None)
            {
                OrderServices orderServices = new OrderServices();
                ProductionServices productionServices = new ProductionServices();
                await orderServices.ChangeOrderStateAsync(orderId, newOrderState);
                productionServices.UpdateProductionStateAsync();
                if (newOrderState == OStateEnum.Finished)
                {
                    OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();
                    orderHangerSectionServices.DeleteAllNonPackedOrderHangersFromOrder(orderId);
                }
            }
        }
    }
}
