using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class DeviceTypeServices
    {
        MyDbContext dbContext = new();

        public DeviceType GetDeviceTypeWithDeviceParts(int deviceTypeId)
        {
            return dbContext.DeviceTypes.Include(dt => dt.DeviceParts).First(dt => dt.Id == deviceTypeId);
        }
    }
}
