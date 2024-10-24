using BL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderHangerPathController : ControllerBase
    {
        private readonly ILogger<OrderHangerPathController> _logger;

        public OrderHangerPathController(ILogger<OrderHangerPathController> logger)
        {
            _logger = logger;
        }

        [HttpDelete(Name = "LetOrderHangerGoToFinal")]
        public async Task LetOrderHangerGoToFinal(int orderHangerId, int sectionId)
        {
            OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();
            orderHangerSectionServices.CancelAllNotEnteredSectionsForOrderHanger(orderHangerId);
            PathServices pathServices = new PathServices();
            SectionServices sectionServices = new SectionServices();
            pathServices.InsertOrderHangerFuturePath(orderHangerId, sectionServices.GetSectionById(sectionId), new List<StationTypeEnum>() { StationTypeEnum.FinalRightSewString1, StationTypeEnum.FinalLeftSewString2 });
            await Task.CompletedTask;
        }
    }
}
