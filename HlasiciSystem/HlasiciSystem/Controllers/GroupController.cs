using Data;
using Data.APIModels;
using Data.DbModels;
using Data.Mapper;
using Microsoft.AspNetCore.Mvc;

namespace HlasiciSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IApplicationMapper mapper;
        private readonly AppDbContext context;

        public GroupController(
            IApplicationMapper mapper, 
            AppDbContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }


        [HttpPost("/create/group")]
        public IActionResult CreateGroup([FromBody] CreateGroup model)
        {
            var group = mapper.ToGroup(model);

            //get user id - will be implemented later

            context.Groups.Add(group);
            context.SaveChanges();
            return Ok();
        }
    }
}
