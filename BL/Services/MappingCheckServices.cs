using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Opc.Ua.Client;
using OpcUaClient;

namespace BL.Services
{
    public class MappingCheckServices
    {
        MyDbContext dbContext = new();

        public async Task CheckMapping()
        {
            List<DeviceType> deviceTypes = await dbContext.DeviceTypes.Include(dt => dt.Devices).Include(dt => dt.Alarms).ToListAsync();

            foreach (var deviceType in deviceTypes)
            {
                foreach (var device in deviceType.Devices)
                {
                    List<string> alarmChildNodes = OpcUaClientManager.Instance.GetChildNodes(device.NodeId + @".""alarms""");
                    List<string> alarmNodes = new List<string>();

                    foreach (var alarmChildNode in alarmChildNodes)
                    {
                        List<string> alarmGrandchildNodes = OpcUaClientManager.Instance.GetChildNodes(alarmChildNode);
                        if (alarmGrandchildNodes.Count > 0)
                        {
                            alarmNodes.AddRange(alarmGrandchildNodes);
                        }
                        else
                        {
                            alarmNodes.AddRange(alarmChildNodes);
                            break;
                        }
                    }

                    alarmNodes.RemoveAll(an => an.Contains("general"));
                    alarmNodes.RemoveAll(an => an.Contains("hw"));
                    alarmNodes.RemoveAll(an => an.Contains("errorCode"));

                    foreach (var alarm in deviceType.Alarms)
                    {
                        if (!alarmNodes.Contains(device.NodeId + "." + alarm.NodeId))
                        {
                            Console.WriteLine("Alarm <" + device.NodeId + "." + alarm.NodeId + "> is not contained in OPC UA alarms.");
                        }
                        else
                        {
                            alarmNodes.Remove(device.NodeId + "." + alarm.NodeId);
                        }
                    }

                    if (alarmNodes.Count > 0)
                    {
                        Console.WriteLine("Alarms <" + string.Join(", ", alarmNodes) + "> are not contained in DB device alarms.");
                    }
                }
            }
        }

        public async Task CheckReading()
        {
            List<DeviceType> deviceTypes = await dbContext.DeviceTypes.Include(dt => dt.Devices).Include(dt => dt.Alarms).ToListAsync();

            int i = 0;

            foreach (var deviceType in deviceTypes)
            {
                foreach (var device in deviceType.Devices)
                {
                    foreach (var alarm in deviceType.Alarms)
                    {
                        await OpcUaClientManager.Instance.ReadValueAsync(device.NodeId + "." + alarm.NodeId, new bool());
                        //Console.WriteLine(i + ": Alarm <" + device.NodeId + "." + alarm.NodeId + "> value: " + await OpcUaClientManager.Instance.ReadValueAsync(device.NodeId + "." + alarm.NodeId, new bool()));
                        i++;
                    }
                }
            }

            await Task.Delay(2000);

            List<Device> devices = await dbContext.Devices.Include(d => d.MonitoredItems).ToListAsync();

            foreach (var device in devices)
            {
                foreach (var monitoredItem in device.MonitoredItems)
                {
                    await OpcUaClientManager.Instance.ReadValueAsync(device.NodeId + "." + monitoredItem.NodeId, new bool());
                    //Console.WriteLine(i + ": Monitored item <" + device.NodeId + "." + monitoredItem.NodeId + "> value: " + await OpcUaClientManager.Instance.ReadValueAsync(device.NodeId + "." + monitoredItem.NodeId, new bool()));
                    i++;
                }
            }
        }
    }
}
