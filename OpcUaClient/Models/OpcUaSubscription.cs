using Opc.Ua.Client;

namespace OpcUaClient.Models
{
    public class OpcUaSubscription
    {
        public required string DisplayName { get; set; }
        public required string NodeId { get; set; }
        public required MonitoredItemNotificationEventHandler Handler { get; set; }
    }
}
