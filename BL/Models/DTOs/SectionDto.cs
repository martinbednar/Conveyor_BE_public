namespace BL.Models.DTOs
{
    public class SectionDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int Capacity { get; set; }
        public required bool Enabled { get; set; }
        public required List<OrderHangerDto> OrderHangers { get; set; }
    }
}
