using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class RfidHeadLogServices
    {
        MyDbContext dbContext = new();

        public void AddLog(RfidHeadLog log)
        {
            dbContext.Add(log);
            dbContext.SaveChanges();
        }

        public void AddLog(int deviceId, SectionDirectionEnum rfidFromDirect, int? orderHangerId, int tagId, int lastTagId)
        {
            AddLog(new RfidHeadLog()
            {
                DeviceId = deviceId,
                RfidFromDirectId = (int)rfidFromDirect,
                OrderHangerId = orderHangerId,
                TagId = tagId,
                LastTagId = lastTagId
            });
        }

        public async Task DeleteAllLogs()
        {
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [RfidHeadLogs]");
        }
    }
}
