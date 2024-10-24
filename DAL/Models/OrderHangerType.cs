namespace DAL.Models
{
    public class OrderHangerType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Deleted { get; set; }
        public virtual ICollection<OrderHanger>? OrderHangers { get; set; }
    }

    public enum OrderHangerTypeEnum
    {
        // PRIVATE - NOT PUBLISHED
    }
}
