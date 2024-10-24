namespace DAL.Models
{
    public class DeviceTypeAlarm
    {
        public int? DeviceTypeId { get; set; }
        public virtual DeviceType? DeviceType { get; set; }
        public required int AlarmId { get; set; }
        public virtual Alarm? Alarm { get; set; }
    }
}
