using BL.Models.DTOs;
using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(ILogger<DevicesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetDevices")]
        public async Task<List<DeviceWebDto>> GetDevices()
        {
            DeviceDtoServices deviceDTOServices = new DeviceDtoServices();
            return await deviceDTOServices.GetAllDevices();
        }
    }
}
