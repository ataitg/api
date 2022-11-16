using Data;
using Data.APIModels;
using Data.DbModels;
using Data.Mapper;
using Data.Enum;
using Identity.Attribute;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpPost("/create/group")]
        public IActionResult CreateGroup([FromBody] CreateGroup model)
        {
            var group = mapper.ToGroup(model);
            group.TeacherId = User.GetUserId();

            context.Groups.Add(group);
            context.SaveChanges();

            return Ok();
        }
    }
}
