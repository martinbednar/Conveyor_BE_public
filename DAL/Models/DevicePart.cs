namespace DAL.Models
{
    public class DevicePart
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string NodeId { get; set; }
        public required virtual ICollection<DeviceType> DeviceTypes { get; set; }
    }
}
