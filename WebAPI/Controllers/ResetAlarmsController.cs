using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResetAlarmsController : ControllerBase
    {
        private readonly ILogger<ResetAlarmsController> _logger;

        public ResetAlarmsController(ILogger<ResetAlarmsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "ResetAlarmsOnDevice")]
        public async Task ResetAlarmsOnDevice([FromQuery] int deviceId)
        {
            AlarmServices alarmServices = new AlarmServices();
            await alarmServices.ResetAlarmsOnDevice(deviceId);
        }

        [HttpPut(Name = "ResetAllAlarms")]
        public async Task ResetAllAlarms()
        {
            AlarmServices alarmServices = new AlarmServices();
            await alarmServices.ResetAllAlarms();
        }
    }
}
