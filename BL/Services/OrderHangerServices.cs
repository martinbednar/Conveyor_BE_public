using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BL.Services
{
    public class OrderHangerServices
    {
        MyDbContext dbContext = new();

        public OrderHanger AddOrderHanger(int orderId, short hangerId, DateTime created, OrderHangerTypeEnum orderHangerType)
        {
            EntityEntry<OrderHanger> newOrderHanger = dbContext.OrderHangers.Add(new OrderHanger()
            {
                OrderId = orderId,
                Started = created,
                HangerId = hangerId,
                OrderHangerTypeId = (int)orderHangerType,
                Sections = new List<OrderHangerSection>(),
                RfidHeadLogs = new List<RfidHeadLog>()
            });
            dbContext.SaveChanges();
            return newOrderHanger.Entity;
        }

        public OrderHanger AddOrderHanger(int orderId, short hangerId, OrderHangerTypeEnum orderHangerType)
        {
            return AddOrderHanger(orderId, hangerId, DateTime.Now, orderHangerType);
        }

        internal OrderHanger GetOrderHanger(int orderHangerId)
        {
            return dbContext.OrderHangers.Find(orderHangerId);
        }

        internal List<OrderHanger> GetNotFinishedOrderHangersByHangerId(short hangerId)
        {
            return dbContext.OrderHangers.Where(oh => (oh.Finished == null) && (oh.Deleted == null)).Where(oh => oh.HangerId == hangerId).ToList();
        }

        internal OrderHanger? GetActiveOrderHangerByHangerId(short hangerId)
        {
            return dbContext.OrderHangers.Where(oh => (oh.Finished == null) && (oh.Deleted == null)).Where(oh => oh.Order.States.OrderBy(s => s.Started).Last().StateId == (int)OStateEnum.Active).OrderByDescending(oh => oh.Id).FirstOrDefault(oh => oh.HangerId == hangerId);
        }

        internal List<OrderHanger> GetOrderHangersForOrder(int orderId)
        {
            return dbContext.OrderHangers.Where(oh => oh.OrderId == orderId).ToList();
        }

        internal void CompleteOrderHanger(int orderHangerId, DateTime timestamp)
        {
            OrderHanger? orderHanger = dbContext.OrderHangers.Find(orderHangerId);
            if (orderHanger != null)
            {
                orderHanger.Finished = timestamp;
                dbContext.SaveChanges();
            }
        }

        public void DeleteOrderHanger(int orderHangerId)
        {
            OrderHanger? orderHanger = dbContext.OrderHangers.Find(orderHangerId);
            if (orderHanger != null)
            {
                orderHanger.Deleted = DateTime.Now;
                dbContext.SaveChanges();
            }
        }

        public async Task DeleteAllOrderHangers()
        {
            await dbContext.OrderHangers.ExecuteDeleteAsync();
        }

        internal void MakeOrderHangerSewn(int orderHangerId, DateTime movedTimestamp)
        {
            dbContext.OrderHangers.Find(orderHangerId).Sewn = movedTimestamp;
            dbContext.SaveChanges();
        }

        internal void MakeOrderHangerPacked(int orderHangerId, DateTime movedTimestamp)
        {
            dbContext.OrderHangers.Find(orderHangerId).Packed = movedTimestamp;
            dbContext.SaveChanges();
        }

        internal List<OrderHanger> GetNonPackedOrderHangersForOrder(int orderId)
        {
            return dbContext.OrderHangers.Where(oh => (oh.Deleted == null) && (oh.Finished == null) && (oh.Packed == null) && (oh.OrderId == orderId)).ToList();
        }
    }
}
