using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MappingCheckController : ControllerBase
    {
        private readonly ILogger<MappingCheckController> _logger;

        public MappingCheckController(ILogger<MappingCheckController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "MappingCheck")]
        public async Task CheckMapping()
        {
            MappingCheckServices mappingCheckServices = new MappingCheckServices();
            await mappingCheckServices.CheckMapping();
        }

        [HttpPut(Name = "CheckReading")]
        public async Task CheckReading()
        {
            MappingCheckServices mappingCheckServices = new MappingCheckServices();
            await mappingCheckServices.CheckReading();
        }
    }
}
