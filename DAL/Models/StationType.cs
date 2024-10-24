namespace DAL.Models
{
    public class StationType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public virtual required ICollection<Section> Stations { get; set; }
    }

    public enum StationTypeEnum
    {
        // PRIVATE - NOT PUBLISHED
    }
}
