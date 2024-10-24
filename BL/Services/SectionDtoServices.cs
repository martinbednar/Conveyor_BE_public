using BL.Models.DTOs;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Services
{
    public class SectionDtoServices
    {
        MyDbContext dbContext = new();

        public async Task<List<SectionDto>> GetAllSectionsDtoAsync()
        {
            List<SectionDto> sections = new List<SectionDto>();
            List<Section> dbSections = await dbContext.Sections.ToListAsync();
            foreach (Section section in dbSections)
            {
                sections.Add(new SectionDto()
                {
                    Id = section.Id,
                    Name = section.Name,
                    Capacity = section.Capacity,
                    Enabled = section.Enabled,
                    OrderHangers = new List<OrderHangerDto>()
                });
            }

            List<OrderHangerSection> dbOrderHangerSections = await dbContext.OrderHangerSections.Where(ohs => (ohs.Left == null) && (ohs.Entered != null)).Include(ohs => ohs.OrderHanger).OrderBy(ohs => ohs.Entered).ToListAsync();
            OrderServices orderServices = new OrderServices();
            foreach (var orderHangerSection in dbOrderHangerSections)
            {
                sections.Find(s => s.Id == orderHangerSection.SectionId).OrderHangers.Add(new OrderHangerDto()
                {
                    Id = orderHangerSection.OrderHangerId,
                    OrderId = orderHangerSection.OrderHanger.OrderId,
                    OrderName = orderServices.GetOrder(orderHangerSection.OrderHanger.OrderId).Name,
                    HangerId = orderHangerSection.OrderHanger.HangerId,
                });
            }

            return sections;
        }

    }
}
