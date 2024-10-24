using BL.Models.DTOs;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class DeviceAlarmLogDtoServices
    {
        MyDbContext dbContext = new();

        public async Task<List<DeviceAlarmLogDto>> GetAllDeviceAlarmLogs()
        {
            List<DeviceAlarmLogDto> deviceAlarmLogs = new List<DeviceAlarmLogDto>();
            List<DeviceAlarmLog> dbDeviceAlarmLogs = await dbContext.DeviceAlarmLogs.Include(dal => dal.Device).Include(dal => dal.Alarm).OrderByDescending(dal => dal.ActiveTo).OrderByDescending(dal => dal.ActiveFrom).Take(1000).ToListAsync();

            foreach (var deviceAlarmLog in dbDeviceAlarmLogs)
            {
                deviceAlarmLogs.Add(new DeviceAlarmLogDto()
                {
                    Id = deviceAlarmLog.Id,
                    ActiveFrom = deviceAlarmLog.ActiveFrom.ToString("dd.MM.yyyy HH:mm:ss"),
                    ActiveTo = (deviceAlarmLog.ActiveTo == null) ? "" : ((DateTime)deviceAlarmLog.ActiveTo).ToString("dd.MM.yyyy HH:mm:ss"),
                    ErrorCode = deviceAlarmLog.ErrorCode,
                    DeviceName = deviceAlarmLog.Device.Name,
                    AlarmName = deviceAlarmLog.Alarm.Name,
                    AlarmDescription = deviceAlarmLog.Alarm.Description
                });
            }

            return deviceAlarmLogs;
        }

        public async Task<List<DeviceAlarmLog>> GetActiveOpcUaActiveAlarms()
        {
            return await dbContext.DeviceAlarmLogs.Where(da => (da.AlarmId == 265) || (da.AlarmId == 266)).Where(da => da.ActiveTo > DateTime.Now).ToListAsync();
        }
    }
}
