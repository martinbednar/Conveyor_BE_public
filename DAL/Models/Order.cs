namespace DAL.Models
{
    public class Order
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int Pieces { get; set; }
        public int PiecesInBox { get; set; }
        public string? Note { get; set; }
        public virtual required ICollection<OrderOState> States { get; set; }
        public virtual ICollection<Section>? Stations { get; set; }
        public virtual ICollection<OrderHanger>? OrderHangers { get; set; }
    }
}
