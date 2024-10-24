using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class LogServices
    {
        MyDbContext dbContext = new();

        public void AddLog(OpcUaLog log)
        {
            dbContext.Add(log);
            dbContext.SaveChanges();
        }

        public void AddLog(string variable, string displayName, string value)
        {
            AddLog(new OpcUaLog()
            {
                DisplayName = displayName,
                DeviceName = displayName,
                Action = variable,
                Value = value
            });
        }

        public void AddLog(string variable, string direction, string deviceName, string value)
        {
            int index = deviceName.Remove(deviceName.Length - 1).LastIndexOf('"');
            string _deviceName = deviceName.Substring(index + 1, 4);

            AddLog(new OpcUaLog()
            {
                DisplayName = direction,
                DeviceName = _deviceName,
                Action = variable,
                Value = value
            });
        }

        public async Task DeleteAllLogs()
        {
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [RfidHeadLogs]");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [OpcUaLogs]");
        }
    }
}
