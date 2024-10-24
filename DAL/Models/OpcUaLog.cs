namespace DAL.Models
{
    public class OpcUaLog
    {
        public int Id { get; set; }
        public required string DisplayName { get; set; }
        public required string DeviceName { get; set; }
        public required string Action { get; set; }
        public required string Value { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
