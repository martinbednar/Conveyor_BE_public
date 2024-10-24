namespace DAL.Models
{
    public class RfidHeadLog
    {
        public int Id { get; set; }
        public required int DeviceId { get; set; }
        public Device? Device { get; set; }
        public required int RfidFromDirectId { get; set; }
        public SectionDirection? RfidFromDirect { get; set; }
        public int? OrderHangerId { get; set; }
        public OrderHanger? OrderHanger { get; set; }
        public required int TagId { get; set; }
        public required int LastTagId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
