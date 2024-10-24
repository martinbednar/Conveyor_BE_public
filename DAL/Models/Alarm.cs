namespace DAL.Models
{
    public class Alarm
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string NodeId { get; set; }
        public required string Description { get; set; }
        public required virtual ICollection<DeviceType> DeviceTypes { get; set; }
    }
}
