namespace BL.Models.DTOs
{
    public class DeviceAlarmLogDto
    {
        public int Id { get; set; }
        public required string ActiveFrom { get; set; }
        public required string ActiveTo { get; set; }
        public int? ErrorCode { get; set; }
        public required string DeviceName { get; set; }
        public required string AlarmName { get; set; }
        public required string AlarmDescription { get; set; }
    }
}
