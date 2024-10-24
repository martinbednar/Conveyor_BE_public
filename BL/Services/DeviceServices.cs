using BL.Mappers;
using BL.Models.Devices;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using OpcUaClient;

namespace BL.Services
{
    public class DeviceServices
    {
        MyDbContext dbContext = new();

        internal List<Device> GetAllDevicesWithMonitoredItems()
        {
            return dbContext.Devices.Include(d => d.MonitoredItems).ToList();
        }

        public Device GetDeviceById(int id)
        {
            return dbContext.Devices.Find(id);
        }

        public async Task SetDeviceEnabledAsync(string nodeId, bool enabled)
        {
            await OpcUaClientManager.Instance.WriteValueAsync(nodeId, enabled);
        }

        internal void TryToMoveHangers()
        {
            // PRIVATE - NOT PUBLISHED
        }

        internal async Task DiscardRequests()
        {
            // PRIVATE - NOT PUBLISHED
        }

        internal void UpdateAlarmGeneral(int deviceId, bool alarmGeneral)
        {
            dbContext.Devices.Find(deviceId).AlarmGeneral = alarmGeneral;
            dbContext.SaveChanges();
        }

        internal void UpdateEnabled(int deviceId, bool enabled)
        {
            dbContext.Devices.Find(deviceId).Enabled = enabled;
            dbContext.SaveChanges();
        }

        public async Task ResetDevice(int deviceId)
        {
            DeviceMapper deviceMapper = new DeviceMapper();
            await deviceMapper.GetDevice(deviceId).DiscardRequestAsync();

            ManualModeServices manualModeServices = new ManualModeServices();
            await manualModeServices.SetManualMode(deviceId, true);
            await Task.Delay(2000);
            await manualModeServices.SetManualMode(deviceId, false);
        }
    }
}
