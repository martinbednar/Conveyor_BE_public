namespace DAL.Models
{
    public class Section
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int Capacity { get; set; }
        public required bool Enabled { get; set; }
        public int? StationTypeId { get; set; }
        public virtual StationType? StationType { get; set; }
        public virtual ICollection<OrderHangerSection>? OrderHangers { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public required int InputDeviceId { get; set; }
        public virtual Device? InputDevice { get; set; }
        public required int OutputDeviceId { get; set; }
        public virtual Device? OutputDevice { get; set; }
        public required int SectionDirectionFromInputDeviceId { get; set; }
        public virtual SectionDirection? SectionDirectionFromInputDevice { get; set; }
        public required int SectionDirectionToOutputDeviceId { get; set; }
        public virtual SectionDirection? SectionDirectionToOutputDevice { get; set; }
    }

    public enum StationEnum
    {
        // PRIVATE - NOT PUBLISHED
    }
}
