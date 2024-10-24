using BL.Models.DTOs;
using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlarmsController : ControllerBase
    {
        private readonly ILogger<AlarmsController> _logger;

        public AlarmsController(ILogger<AlarmsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetAllDeviceAlarmLogs")]
        public async Task<List<DeviceAlarmLogDto>> GetAllDeviceAlarmLogs()
        {
            DeviceAlarmLogDtoServices deviceAlarmLogDtoServices = new DeviceAlarmLogDtoServices();
            return await deviceAlarmLogDtoServices.GetAllDeviceAlarmLogs();
        }
    }
}
