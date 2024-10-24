namespace DAL.Models
{
    public class Device
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string NodeId { get; set; }
        public bool AlarmGeneral { get; set; } = false;
        public bool Enabled { get; set; } = true;
        public virtual required ICollection<Section> PreviousSections { get; set; }
        public virtual required ICollection<Section> NextSections { get; set; }
        public virtual required ICollection<MonitoredItem> MonitoredItems { get; set; }
        public required int DeviceTypeId { get; set; }
        public virtual DeviceType? DeviceType { get; set; }
        public virtual required ICollection<DeviceAlarmLog> Alarms { get; set; }
        public virtual required ICollection<RfidHeadLog> RfidHeadLogs { get; set; }
    }
}
