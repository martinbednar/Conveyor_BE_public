using BL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(ILogger<DeviceController> logger)
        {
            _logger = logger;
        }

        [HttpPut(Name = "SetDeviceEnabled")]
        public async Task SetDeviceEnabled(int deviceId, bool enabled)
        {
            DeviceServices deviceServices = new DeviceServices();
            Device device = deviceServices.GetDeviceById(deviceId);
            await deviceServices.SetDeviceEnabledAsync(device.NodeId + @".""i"".""enabled""", enabled);
        }

        [HttpDelete(Name = "ResetDevice")]
        public async Task ResetDevice(int deviceId)
        {
            DeviceServices deviceServices = new DeviceServices();
            await deviceServices.ResetDevice(deviceId);
        }
    }
}
