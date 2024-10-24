using DAL.Models;
using OpcUaClient;

namespace BL.Services
{
    public class ProductionServices
    {
        private async void SetProductionAsync(bool value)
        {
            await OpcUaClientManager.Instance.WriteValueAsync(@"ns=3;s=""systemStateDB"".""i"".""productionStarted""", value);
        }

        private void UpdateProductionStateAsync(List<Order> activeOrders)
        {
            if (activeOrders.Count > 0)
            {
                SetProductionAsync(true);
            }
            else
            {
                SetProductionAsync(false);
            }
        }



        private async void SetStationEnabledAsync(StationEnum station, bool value)
        {
            DeviceServices deviceServices = new DeviceServices();
            SectionServices sectionServices = new SectionServices();
            Section section = sectionServices.GetSectionById((int)station);
            Device device = deviceServices.GetDeviceById(section.OutputDeviceId);
            await OpcUaClientManager.Instance.WriteValueAsync(device.NodeId + @".""i"".""enabled""", value);
        }

        private Order? GetActiveOrderOnStation(List<Order> activeOrders, StationEnum station)
        {
            return activeOrders.Where(o => o.Stations.Any(s => s.Id == (int)station)).FirstOrDefault();
        }

        private void UpdateStationsEnabledAsync(List<StationEnum> stations, List<Order> activeOrders)
        {
            foreach (StationEnum station in stations)
            {
                Order? activeOrderOnStation = GetActiveOrderOnStation(activeOrders, station);
                SetStationEnabledAsync(station, activeOrderOnStation != null);
            }
        }


        private async void SetTubingRemainingHangersToProduceAsync(StationEnum station, int value)
        {
            DeviceServices deviceServices = new DeviceServices();
            SectionServices sectionServices = new SectionServices();
            Section section = sectionServices.GetSectionById((int)station);
            Device device = deviceServices.GetDeviceById(section.OutputDeviceId);
            await OpcUaClientManager.Instance.WriteValueAsync(device.NodeId + @".""i"".""piecesToFinish""", value);
        }

        public void UpdateRemaingHangersToProduceAsync(List<StationEnum> tubings)
        {
            OrderHangerServices orderHangerServices = new OrderHangerServices();
            OrderServices orderServices = new OrderServices();

            foreach (StationEnum tubing in tubings)
            {
                Order? activeOrderOnTubing = orderServices.GetActiveOrderForStation(tubing);
                if (activeOrderOnTubing != null)
                {
                    List<OrderHanger> alreadyProducedOrderHangers = orderHangerServices.GetOrderHangersForOrder(activeOrderOnTubing.Id);
                    int remaingHangersToProduce = activeOrderOnTubing.Pieces - alreadyProducedOrderHangers.Count();
                    SetTubingRemainingHangersToProduceAsync(tubing, remaingHangersToProduce);
                }
            }
        }



        public async void UpdateProductionStateAsync()
        {
            OrderServices orderServices = new OrderServices();
            List<Order> activeOrders = await orderServices.GetAllActiveOrdersWithStationsAsync();

            List<StationEnum> allTubings = new List<StationEnum>()
            {
                // PRIVATE - NOT PUBLISHED
            };

            List<StationEnum> allSewings = new List<StationEnum>()
            {
                // PRIVATE - NOT PUBLISHED
            };


            UpdateStationsEnabledAsync(allTubings, activeOrders);
            UpdateStationsEnabledAsync(allSewings, activeOrders);
            UpdateRemaingHangersToProduceAsync(allTubings);
            UpdateProductionStateAsync(activeOrders);
        }
    }
}
