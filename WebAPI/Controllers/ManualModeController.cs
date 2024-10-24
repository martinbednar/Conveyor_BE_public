using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManualModeController : ControllerBase
    {
        private readonly ILogger<ManualModeController> _logger;

        public ManualModeController(ILogger<ManualModeController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "SetManualMode")]
        public async Task SetManualMode([FromQuery] int deviceId, [FromQuery] bool value)
        {
            ManualModeServices manualModeServices = new ManualModeServices();
            await manualModeServices.SetManualMode(deviceId, value);
        }

        [HttpPut(Name = "SetImpulsToVariable")]
        public async Task SetImpulsToVariable([FromQuery] int deviceId, [FromQuery] string devicePart, [FromQuery] string variable)
        {
            ManualModeServices manualModeServices = new ManualModeServices();
            await manualModeServices.SetVariableValue(deviceId, devicePart, variable, true);
            await Task.Delay(1000);
            await manualModeServices.SetVariableValue(deviceId, devicePart, variable, false);
        }

        [HttpPost(Name = "SetVariableValue")]
        public async Task SetVariableValue([FromQuery] int deviceId, [FromQuery] string devicePart, [FromQuery] string variable, [FromQuery] bool value)
        {
            ManualModeServices manualModeServices = new ManualModeServices();
            await manualModeServices.SetVariableValue(deviceId, devicePart, variable, value);
        }
    }
}
