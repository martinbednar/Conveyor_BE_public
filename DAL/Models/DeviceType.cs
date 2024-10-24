namespace DAL.Models
{
    public class DeviceType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required virtual ICollection<Device> Devices { get; set; }
        public required virtual ICollection<Alarm> Alarms { get; set; }
        public required virtual ICollection<DevicePart> DeviceParts { get; set; }
    }
}
