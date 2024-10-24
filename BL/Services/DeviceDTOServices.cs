using BL.Models.DTOs;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class DeviceDtoServices
    {
        MyDbContext dbContext = new();

        public async Task<List<DeviceWebDto>> GetAllDevices()
        {
            List<DeviceWebDto> devicesDTO = new List<DeviceWebDto>();
            List<Device> dbDevices = await dbContext.Devices.Include(d => d.DeviceType).ToListAsync();
            List<DeviceAlarmLog> dbDeviceAlarms = await dbContext.DeviceAlarmLogs.Include(da => da.Alarm).AsNoTracking().Where(da => da.ActiveTo == null).ToListAsync();

            foreach (var device in dbDevices)
            {
                devicesDTO.Add(new DeviceWebDto()
                {
                    Id = device.Id,
                    Type = device.DeviceType?.Name ?? "Unknown",
                    Enabled = device.Enabled,
                    AlarmGeneral = device.AlarmGeneral,
                    Alarms = dbDeviceAlarms.Where(da => da.DeviceId == device.Id).ToList()
                });
            }

            return devicesDTO;
        }
    }
}
