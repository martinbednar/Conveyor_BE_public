namespace DAL.Models
{
    public class DeviceMonitoredItem
    {
        public required int DeviceId { get; set; }
        public virtual Device? Device { get; set; }
        public required int MonitoredItemId { get; set; }
        public virtual MonitoredItem? MonitoredItem { get; set; }
    }
}
