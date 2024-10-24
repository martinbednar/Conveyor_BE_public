namespace DAL.Models
{
    public class OrderHangerSection
    {
        public int Id { get; set; }
        public DateTime? Entered { get; set; }
        public DateTime? Left { get; set; }
        public int Index { get; set; }
        public required int OrderHangerId { get; set; }
        public virtual OrderHanger? OrderHanger { get; set; }
        public required int SectionId { get; set; }
        public virtual Section? Section { get; set; }
    }
}
