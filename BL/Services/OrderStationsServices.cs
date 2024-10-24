using BL.Models.DTOs;
using DAL.Data;
using DAL.Models;

namespace BL.Services
{
    public class OrderStationsServices
    {
        MyDbContext dbContext = new();

        internal List<Section> GetActiveOrderStations(int orderId, StationTypeEnum stationType)
        {
            if (stationType == StationTypeEnum.Tubing)
            {
                return dbContext.Sections.Where(s => s.StationTypeId == (int)stationType).Where(s => s.Orders.Any(o => o.States.OrderBy(s => s.Id).Last().StateId == (int)OStateEnum.Active)).ToList();
            }
            else {
                OrderServices orderServices = new OrderServices();
                Order orderWithStates = orderServices.GetOrderWithStates(orderId);
                if (orderWithStates.States.OrderBy(s => s.Started).Last().StateId == (int)OStateEnum.Active)
                {
                    return dbContext.Sections.Where(s => s.StationTypeId == (int)stationType).Where(s => s.Orders.Any(o => o.Id == orderId)).ToList();
                }
                else
                {
                    return new List<Section>();
                }
            }
        }

        public async Task<string> ValidateOrderStationsAsync(OrderDto order)
        {
            OrderServices orderServices = new OrderServices();
            List<Order> activeOrdersWithStations = await orderServices.GetAllOrdersInStateWithStationsAsync(OStateEnum.Active);
            activeOrdersWithStations.RemoveAll(o => o.Id == order.Id);
            foreach (Order activeOrder in activeOrdersWithStations)
            {
                // PRIVATE - NOT PUBLISHED
            }
            return "";
        }
    }
}
