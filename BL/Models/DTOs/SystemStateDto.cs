namespace BL.Models.DTOs
{
    public class SystemStateDto
    {
        public required bool IsOpcUaCommunicationGood { get; set; } = true;
        public required string OpcUaCommunicationErrorMessage { get; set; } = "";
    }
}
