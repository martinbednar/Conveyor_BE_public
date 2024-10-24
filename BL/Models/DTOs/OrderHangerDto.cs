namespace BL.Models.DTOs
{
    public class OrderHangerDto
    {
        public required int Id { get; set; }
        public required int OrderId { get; set; }
        public required string OrderName { get; set; }
        public required int HangerId { get; set; }
    }
}
