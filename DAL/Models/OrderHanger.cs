namespace DAL.Models
{
    public class OrderHanger
    {
        public int Id { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Sewn { get; set; }
        public DateTime? Packed { get; set; }
        public DateTime? Finished { get; set; }
        public DateTime? Deleted { get; set; }
        public required int OrderId { get; set; }
        public virtual Order? Order { get; set; }
        public required int HangerId { get; set; }
        public required int OrderHangerTypeId { get; set; }
        public virtual OrderHangerType? OrderHangerType { get; set; }
        public virtual required ICollection<OrderHangerSection> Sections { get; set; }
        public virtual required ICollection<RfidHeadLog> RfidHeadLogs { get; set; }
    }
}
