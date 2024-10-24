using BL.Models.DTOs;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class OrderDtoServices
    {
        MyDbContext dbContext = new();

        public async Task<List<OrderDto>> GetAllOrdersDtoAsync()
        {
            List<OrderDto> orders = new List<OrderDto>();
            List<Order> dbOrders = await dbContext.Orders.Include(o => o.States).Include(o => o.OrderHangers).ToListAsync();
            foreach (Order order in dbOrders)
            {
                orders.Add(new OrderDto()
                {
                    Id = order.Id,
                    Name = order.Name,
                    Pieces = order.Pieces,
                    PiecesAlreadyMade = order.OrderHangers.Where(oh => oh.Started != null).Count(),
                    PiecesAlreadySewn = order.OrderHangers.Where(oh => oh.Sewn != null).Count(),
                    PiecesAlreadyPacked = order.OrderHangers.Where(oh => oh.Packed != null).Count(),
                    PiecesInBox = order.PiecesInBox,
                    Note = order.Note,
                    StateId = order.States.Where(s => s.Finished == null).OrderBy(s => s.Started).Last().StateId
                });
            }

            return orders;
        }

        public async Task<OrderDto> GetOrderDtoAsync(int id)
        {
            Order? order = await dbContext.Orders.Include(o => o.States).Include(o => o.Stations).Include(o => o.OrderHangers).FirstOrDefaultAsync(order => order.Id == id);

            OrderDto orderDto = new OrderDto()
            {
                Name = "null",
                Pieces = 0,
                StateId = 0
            };

            if (order != null)
            {
                orderDto.Id = order.Id;
                orderDto.Name = order.Name;
                orderDto.Pieces = order.Pieces;
                orderDto.PiecesAlreadyMade = order.OrderHangers.Where(oh => oh.Started != null).Count();
                orderDto.PiecesAlreadySewn = order.OrderHangers.Where(oh => oh.Sewn != null).Count();
                orderDto.PiecesAlreadyPacked = order.OrderHangers.Where(oh => oh.Packed != null).Count();
                orderDto.PiecesInBox = order.PiecesInBox;
                orderDto.Note = order.Note;
                orderDto.StateId = order.States.Where(s => s.Finished == null).OrderBy(s => s.Started).Last().StateId;

                // PRIVATE - NOT PUBLISHED
            }

            return orderDto;
        }

        public async Task<int> AddOrderAsync(OrderDto orderDto)
        {
            Order order = new Order()
            {
                Name = orderDto.Name,
                Pieces = orderDto.Pieces,
                PiecesInBox = orderDto.PiecesInBox,
                Note = orderDto.Note,
                States = new List<OrderOState> {
                    new OrderOState()
                    {
                        StateId = (int)OStateEnum.Planned,
                        Started = DateTime.Now
                    }
                },
                Stations = new List<Section>()
            };

            // PRIVATE - NOT PUBLISHED

            var newOrder = await dbContext.AddAsync(order);

            await dbContext.SaveChangesAsync();

            return newOrder.Entity.Id;
        }

        public async Task UpdateOrderAsync(OrderDto orderDto)
        {
            Order? order = await dbContext.Orders.Include(o => o.Stations).FirstOrDefaultAsync(order => order.Id == orderDto.Id);

            if (order != null)
            {
                order.Name = orderDto.Name;
                order.Pieces = orderDto.Pieces;
                order.PiecesInBox = orderDto.PiecesInBox;
                order.Note = orderDto.Note;
                order.Stations = new List<Section>();

                // PRIVATE - NOT PUBLISHED

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<OrderDto?> GetActiveOrderDtoForStationAsync(StationEnum station)
        {
            OrderServices orderServices = new OrderServices();
            Order? activeOrder = orderServices.GetActiveOrderForStation(station);
            if (activeOrder != null)
            {
                return await GetOrderDtoAsync(activeOrder.Id);
            }
            else
            {
                return null;
            }
        }
    }
}
