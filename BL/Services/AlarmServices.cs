using DAL.Data;
using DAL.Models;
using Microsoft.IdentityModel.Tokens;
using OpcUaClient;

namespace BL.Services
{
    public class AlarmServices
    {
        MyDbContext dbContext = new();

        public List<Alarm> GetAlarmsOfDevice(Device device)
        {
            return dbContext.Alarms.Where(a => a.DeviceTypes.Any(dt => dt.Id == device.DeviceTypeId)).ToList();
        }

        private async Task SetResetAllAlarms(bool value)
        {
            await OpcUaClientManager.Instance.WriteValueAsync(
                @"ns=3;s=""alarmsDB"".""resetAlarms""",
                value
            );
        }

        private async Task SetResetAlarmsOnDevice(int deviceId, bool value)
        {
            DeviceServices deviceServices = new DeviceServices();
            Device device = deviceServices.GetDeviceById(deviceId);
            await OpcUaClientManager.Instance.WriteValueAsync(
                device.NodeId + @".""i"".""resetAlarms""",
                value
            );
        }

        public async Task ResetAllAlarms()
        {
            await SetResetAllAlarms(true);
            await Task.Delay(2000);
            await SetResetAllAlarms(false);
        }

        public async Task ResetAlarmsOnDevice(int deviceId)
        {
            await SetResetAlarmsOnDevice(deviceId, true);
            await Task.Delay(2000);
            await SetResetAlarmsOnDevice(deviceId, false);
        }

        public async Task ReadAlarmsOnDeviceAsync(int deviceId)
        {
            DeviceServices deviceServices = new DeviceServices();
            Device device = deviceServices.GetDeviceById(deviceId);
            List<Alarm> allPossibleDeviceAlarms = dbContext.Alarms.Where(a => a.DeviceTypes.Any(dt => dt.Id == device.DeviceTypeId)).ToList();
            DateTime nowTimestamp = DateTime.Now;
            foreach (Alarm deviceAlarm in allPossibleDeviceAlarms)
            {
                string node = device.NodeId + @"." + deviceAlarm.NodeId;
                bool isAlarmActive = await OpcUaClientManager.Instance.ReadValueAsync(node, new bool());
                if (isAlarmActive)
                {
                    bool alarmIsNotInLogYet = dbContext.DeviceAlarmLogs.Where(dal => dal.DeviceId == deviceId && dal.AlarmId == deviceAlarm.Id && dal.ActiveTo == null).IsNullOrEmpty();
                    if (alarmIsNotInLogYet)
                    {
                        dbContext.DeviceAlarmLogs.Add(new DeviceAlarmLog
                        {
                            DeviceId = deviceId,
                            AlarmId = deviceAlarm.Id,
                            ActiveFrom = nowTimestamp
                        });
                    }
                }
                else
                {
                    dbContext.DeviceAlarmLogs.Where(dal => dal.DeviceId == deviceId && dal.AlarmId == deviceAlarm.Id && dal.ActiveTo == null).ToList()
                        .ForEach(dal => dal.ActiveTo = nowTimestamp);
                }
            }
            dbContext.SaveChanges();
        }

        internal async void CheckAlarms()
        {
            List<int> deviceIdsToCheck = dbContext.DeviceAlarmLogs.Where(dal => dal.ActiveTo == null).Select(dal => dal.DeviceId).Distinct().ToList();
            foreach (int deviceId in deviceIdsToCheck)
            {
                await ReadAlarmsOnDeviceAsync(deviceId);
            }
        }

        internal void DiscardAllActiveAlarmsOnDeviceAsync(int deviceId)
        {
            DateTime nowTimestamp = DateTime.Now;
            dbContext.DeviceAlarmLogs.Where(dal => dal.DeviceId == deviceId && dal.ActiveTo == null).ToList()
                        .ForEach(dal => dal.ActiveTo = nowTimestamp);
            dbContext.SaveChanges();
        }

        internal void AddNewAlarmLog(int deviceId, int alarmId)
        {
            dbContext.Add(new DeviceAlarmLog
            {
                DeviceId = deviceId,
                AlarmId = alarmId,
                ActiveFrom = DateTime.Now,
                ActiveTo = DateTime.Now.AddSeconds(10)
            });
            dbContext.SaveChanges();
        }
    }
}
