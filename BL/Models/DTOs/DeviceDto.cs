namespace BL.Models.DTOs
{
    public abstract class DeviceDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string NodeId { get; set; }
        public required ICollection<DAL.Models.MonitoredItem> MonitoredItems { get; set; }
    }
}
