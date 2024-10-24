namespace DAL.Models
{
    public class DeviceTypePart
    {
        public int? DeviceTypeId { get; set; }
        public virtual DeviceType? DeviceType { get; set; }
        public required int DevicePartId { get; set; }
        public virtual DevicePart? DevicePart { get; set; }
    }
}
