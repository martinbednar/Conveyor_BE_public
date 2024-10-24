using DAL.Data;
using DAL.Models;
using OpcUaClient;

namespace BL.Services
{
    public class ManualModeServices
    {
        MyDbContext dbContext = new();

        public async Task SetManualMode(int deviceId, bool value)
        {
            DeviceServices deviceServices = new DeviceServices();
            Device device = deviceServices.GetDeviceById(deviceId);
            DeviceTypeServices deviceTypeServices = new DeviceTypeServices();
            DeviceType deviceType = deviceTypeServices.GetDeviceTypeWithDeviceParts(device.DeviceTypeId);

            if (deviceType.DeviceParts.Count() == 0)
            {
                await OpcUaClientManager.Instance.WriteValueAsync(
                    device.NodeId + @".""man"".""active""",
                    value
                );

            }
            else
            {
                List<string> nodes = new List<string>();
                foreach (DevicePart devicePart in deviceType.DeviceParts)
                {
                    nodes.Add(device.NodeId + @".""man"".""" + devicePart.NodeId + @""".""active""");
                }
                await OpcUaClientManager.Instance.WriteValuesAsync(nodes, value);
            }
        }

        public async Task SetVariableValue(int deviceId, string devicePart, string variable, bool value)
        {
            DeviceServices deviceServices = new DeviceServices();
            Device device = deviceServices.GetDeviceById(deviceId);
            if ((devicePart != "null") && (devicePart != null) && (devicePart != ""))
            {
                if (devicePart == "driver")
                {
                    await OpcUaClientManager.Instance.WriteValueAsync(
                        device.NodeId + @".""control"".""" + devicePart + @""".""" + variable + @"""",
                        value
                    );
                }
                else
                {
                    await OpcUaClientManager.Instance.WriteValueAsync(
                        device.NodeId + @".""man"".""" + devicePart + @""".""" + variable + @"""",
                        value
                    );
                }
            }
            else
            {
                await OpcUaClientManager.Instance.WriteValueAsync(
                    device.NodeId + @".""man"".""" + variable + @"""",
                    value
                );
            }
        }
    }
}
