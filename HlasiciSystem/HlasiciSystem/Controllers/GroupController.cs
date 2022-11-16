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

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpGet("/delete/group/{id}")]
        public IActionResult DeleteGroup([FromRoute] string groupId)
        {
            var group = context.Groups.FirstOrDefault(x => x.Id.ToString() == groupId);
            if (group == null)
            {
                return BadRequest();
            }

            if (group.TeacherId != User.GetUserId())
            {
                return Forbid();
            }

            context.Groups.Remove(group);
            context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpGet("/activate/group/{id}")]
        public IActionResult ActivateGroup([FromRoute] string groupId)
        {
            var group = context.Groups.FirstOrDefault(x => x.Id.ToString() == groupId);
            if (group == null)
            {
                return BadRequest();
            }

            if(group.TeacherId != User.GetUserId())
            {
                return Forbid();
            }

            group.IsActive = true;
            context.Groups.Update(group);
            context.SaveChanges();

            return Ok();
        }
    }
}
