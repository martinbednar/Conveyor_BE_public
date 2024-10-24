using DAL.Models;

namespace BL.Models.DTOs
{
    public class DeviceWebDto
    {
        public required int Id { get; set; }
        public required string Type { get; set; }
        public required bool Enabled { get; set; }
        public required bool AlarmGeneral { get; set; }
        public required List<DeviceAlarmLog> Alarms { get; set; }
    }
}
