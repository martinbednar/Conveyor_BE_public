namespace DAL.Models
{
    public class DeviceAlarmLog
    {
        public int Id { get; set; }
        public required DateTime ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        public int? ErrorCode { get; set; }
        public required int DeviceId { get; set; }
        public virtual Device? Device { get; set; }
        public required int AlarmId { get; set; }
        public virtual Alarm? Alarm { get; set; }
    }
}
