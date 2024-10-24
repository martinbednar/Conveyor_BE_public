namespace BL.Models.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int Pieces { get; set; }
        public int PiecesAlreadyMade { get; set; }
        public int PiecesAlreadySewn { get; set; }
        public int PiecesAlreadyPacked { get; set; }
        public int PiecesInBox { get; set; }
        public string? Note { get; set; }
        public required int StateId { get; set; }
        // PRIVATE - NOT PUBLISHED
    }
}
