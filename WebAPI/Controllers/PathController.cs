using BL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PathController : ControllerBase
    {
        private readonly ILogger<PathController> _logger;

        public PathController(ILogger<PathController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Path")]
        public async Task<List<Section>> GetPath(int fromId, int toId)
        {
            SectionServices sectionService = new SectionServices();
            PathServices pathServices = new PathServices();

            return pathServices.GetPath(sectionService.GetSectionById(fromId), sectionService.GetSectionById(toId));
        }
    }
}
