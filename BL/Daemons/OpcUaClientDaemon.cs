using BL.Mappers;
using BL.Services;
using DAL.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpcUaClient;
using OpcUaClient.Models;

namespace BL.Daemons
{
    public class OpcUaClientDaemon : BackgroundService
    {
        private readonly ILogger<OpcUaClientDaemon> _logger;

        public OpcUaClientDaemon(ILogger<OpcUaClientDaemon> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _startOpcUaClientDaemon(stoppingToken);
        }

        private async Task _startOpcUaClientDaemon(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OPC UA Daemon routine started.");

            DeviceServices deviceServices = new DeviceServices();
            List<Device> devices = deviceServices.GetAllDevicesWithMonitoredItems();

            MonitoredItemMapper monitoredItemMapper = new MonitoredItemMapper();

            List<OpcUaSubscription> opcUaSubscriptions = new List<OpcUaSubscription>();

            foreach (Device device in devices)
            {
                foreach (MonitoredItem monitoredItem in device.MonitoredItems)
                {
                    opcUaSubscriptions.Add(new OpcUaSubscription()
                    {
                        DisplayName = device.Name + "." + monitoredItem.Name,
                        NodeId = device.NodeId + "." + monitoredItem.NodeId,
                        Handler = monitoredItemMapper.GetSubscriptionHandler(device.Id, monitoredItem.Id)
                    });
                }
            }

            while (true)
            {
                try
                {
                    await deviceServices.DiscardRequests();
                    break; // Exit the loop when all request successfully cleaned up.
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("OPC UA Client: Failed to clean up requests.");
                }
            }

            await Task.Delay(5000, cancellationToken);

            while (true)
            {
                try
                {
                    OpcUaClientManager.Instance.AddListeners(opcUaSubscriptions);
                    break; // Exit the loop when listeners successfully added.
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("OPC UA Client: Failed to add listeners.");
                }
            }

            await Task.Delay(5000, cancellationToken);

            ProductionServices productionServices = new ProductionServices();
            productionServices.UpdateProductionStateAsync();

            AlarmServices alarmServices = new AlarmServices();

            ConnectionServices connectionServices = new ConnectionServices();

            await Task.Delay(5000, cancellationToken);

            while (true)
            {
                deviceServices.TryToMoveHangers();
                await Task.Delay(1000, cancellationToken);
                deviceServices.TryToMoveHangersOnSpms();
                await Task.Delay(1000, cancellationToken);
                connectionServices.InvertLiveBitAsync();
            }
        }
    }
}
