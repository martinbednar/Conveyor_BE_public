namespace DAL.Models
{
    public class OState
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Deleted { get; set; }
        public virtual required ICollection<OrderOState> Orders { get; set; }
    }

    public enum OStateEnum
    {
        // PRIVATE - NOT PUBLISHED
    }
}
