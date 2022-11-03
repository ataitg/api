using Data;
using Data.APIModels;
using Data.DbModels;
using Data.Mapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HlasiciSystem.Controllers
{
    [ApiController]
    [Route("[controler]")]
    public class ClassController : ControllerBase
    {
        private readonly IApplicationMapper mapper;
        private readonly AppDbContext context;
        public ClassController(IApplicationMapper mapper, AppDbContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        [HttpPost]
        public IActionResult CreateClass([FromBody] CreateClass model)
        {
            var classs = mapper.ToClass(model);
            context.Classes.Add(classs);
            context.SaveChanges();
            return Ok();
        }
    }
}
