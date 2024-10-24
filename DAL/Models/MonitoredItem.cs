namespace DAL.Models
{
    public class MonitoredItem
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string NodeId { get; set; }
        public required virtual ICollection<Device> Devices { get; set; }
    }
}
