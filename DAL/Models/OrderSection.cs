namespace DAL.Models
{
    public class OrderSection
    {
        public required int OrderId { get; set; }
        public virtual required Order Order { get; set; }
        public required int StationId { get; set; }
        public virtual required Section Station { get; set; }
    }
}
