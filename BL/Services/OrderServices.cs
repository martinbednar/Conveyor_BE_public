using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class OrderServices
    {
        MyDbContext dbContext = new();

        public async Task<Order> GetOrderAsync(int id)
        {
            return await dbContext.Orders.Include(o => o.Stations).FirstOrDefaultAsync(order => order.Id == id);
        }

        public async Task<Order> GetOrderWithStatesAsync(int id)
        {
            return await dbContext.Orders.Include(o => o.States).FirstOrDefaultAsync(order => order.Id == id);
        }

        public Order GetOrderWithStates(int id)
        {
            return dbContext.Orders.Include(o => o.States).FirstOrDefault(order => order.Id == id);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await dbContext.Orders.ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersInStateWithStationsAsync(OStateEnum orderState)
        {
            return await dbContext.Orders.Include(o => o.Stations).Where(order => order.States.OrderBy(state => state.Started).Last().StateId == (int)orderState).ToListAsync();
        }

        public async Task<List<Order>> GetAllActiveOrdersWithStationsAsync()
        {
            return await GetAllOrdersInStateWithStationsAsync(OStateEnum.Active);
        }

        public List<Order> GetAllOrdersInStateWithStations(OStateEnum orderState)
        {
            return dbContext.Orders.Include(o => o.Stations).Where(order => order.States.OrderBy(state => state.Started).Last().StateId == (int)orderState).ToList();
        }

        public List<Order> GetAllActiveOrdersWithStations()
        {
            return GetAllOrdersInStateWithStations(OStateEnum.Active);
        }

        public async Task ChangeOrderStateAsync(int orderId, OStateEnum orderNewState)
        {
            // 1 is the id of the NoOrder order, which is a special order that is always open (no need to close it)
            if (orderId != 1)
            {
                Order orderDb = await GetOrderWithStatesAsync(orderId);

                DateTime now = DateTime.Now;

                orderDb.States.OrderBy(state => state.Started).Last().Finished = now;
                orderDb.States.Add(new OrderOState()
                {
                    Started = now,
                    OrderId = orderDb.Id,
                    StateId = (int)orderNewState,
                });

                await dbContext.SaveChangesAsync();
            }
        }

        public void ChangeOrderState(int orderId, OStateEnum orderNewState)
        {
            // 1 is the id of the NoOrder order, which is a special order that is always open (no need to close it)
            if (orderId != 1)
            {
                Order orderDb = GetOrderWithStates(orderId);

                DateTime now = DateTime.Now;

                orderDb.States.OrderBy(state => state.Started).Last().Finished = now;
                orderDb.States.Add(new OrderOState()
                {
                    Started = now,
                    OrderId = orderDb.Id,
                    StateId = (int)orderNewState,
                });

                dbContext.SaveChanges();
            }
        }

        public List<Section> GetOrderStations(int orderId)
        {
            return dbContext.Sections.Where(s => s.Orders.Any(o => o.Id == orderId)).ToList();
        }

        public async Task UpdateOrderStationsAsync(int orderId, List<Section> stations)
        {
            Order orderDb = await GetOrderAsync(orderId);

            orderDb.Stations = stations;

            await dbContext.SaveChangesAsync();
        }

        public Order? GetActiveOrderForStation(StationEnum station)
        {
            List<Order> ordersOnStation = dbContext.Orders.Where(o => o.Stations.Any(s => s.Id == (int)station)).Include(o => o.States).ToList();
            if (ordersOnStation.Count > 0)
            {
                List<Order> activeOrdersOnStation = ordersOnStation.Where(o => o.States.OrderBy(state => state.Started).Last().StateId == (int)OStateEnum.Active).OrderBy(o => o.Id).ToList();
                if (activeOrdersOnStation.Count > 0)
                {
                    return activeOrdersOnStation.Last();
                }
            }
            return null;
        }

        internal Order GetOrder(int orderId)
        {
            return dbContext.Orders.Find(orderId);
        }

        internal void CloseOrderIfFinished(int orderId)
        {
            Order order = GetOrder(orderId);
            int numberOfPackedHangersOfOrder = dbContext.OrderHangers.Where(oh => oh.OrderId == orderId).Where(oh => oh.Packed != null).Count();
            if (numberOfPackedHangersOfOrder >= order.Pieces)
            {
                ChangeOrderState(orderId, OStateEnum.Finished);
                OrderHangerSectionServices orderHangerSectionServices = new OrderHangerSectionServices();
                orderHangerSectionServices.DeleteAllNonPackedOrderHangersFromOrder(orderId);
                ProductionServices productionServices = new ProductionServices();
                productionServices.UpdateProductionStateAsync();
            }
        }
    }
}
