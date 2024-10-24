namespace DAL.Models
{
    public class OrderOState
    {
        public int Id { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Finished { get; set; }
        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }
        public int StateId { get; set; }
        public virtual OState? State { get; set; }
    }
}
