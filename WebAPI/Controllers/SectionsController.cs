using BL.Models.DTOs;
using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SectionsController : ControllerBase
    {
        private readonly ILogger<SectionsController> _logger;

        public SectionsController(ILogger<SectionsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Sections")]
        public async Task<List<SectionDto>> GetSections()
        {
            SectionDtoServices SectionDtoServices = new SectionDtoServices();
            return await SectionDtoServices.GetAllSectionsDtoAsync();
        }
    }
}
