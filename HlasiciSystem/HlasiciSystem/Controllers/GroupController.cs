using Data;
using Data.APIModels;
using Data.DbModels;
using Data.Mapper;
using Data.Enum;
using Identity.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Data.VModels;

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

            if (group.TeacherId != User.GetUserId())
            {
                return Forbid();
            }

            group.IsActive = true;
            context.Groups.Update(group);
            context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpGet("/deactivate/group/{id}")]
        public IActionResult DeactivateGroup([FromRoute] string groupId)
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

            group.IsActive = false;
            context.Groups.Update(group);
            context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpPost("/rename/group/{id}")]
        public IActionResult RenameGroup([FromRoute] string groupId, [FromBody] RenameGroup model)
        {
            var group = context.Groups.FirstOrDefault(x => x.Id.ToString() == groupId);
            if(group== null)
            {
                return BadRequest();
            }

            if(group.TeacherId != User.GetUserId())
            {
                return Forbid();
            }

            group.Name = model.Name;
            context.Groups.Update(group);
            context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpPost("/add/users/group/{id}")]
        public IActionResult AddUsersToGroup([FromRoute] string groupId, [FromBody] List<string> userIds)
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

            userIds.ForEach(userId =>
            {
                var user = context.Users.FirstOrDefault(x => x.Id.ToString() == userId);
                if (user != null)
                {
                    context.UserGroups.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        GroupId = group.Id,
                        UserId = user.Id,

                    });
                    context.SaveChanges();
                }
            });

            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpDelete("/remove/users/group/{id}")]
        public IActionResult RemoveUsersFromGroup([FromRoute] string groupId, [FromBody] List<string> userIds)
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

            userIds.ForEach(userId =>
            {
                var userGroup = context.UserGroups.FirstOrDefault(x => x.UserId.ToString() == userId && x.GroupId.ToString() == groupId);
                if (userGroup != null)
                {
                    context.UserGroups.Remove(userGroup);
                    context.SaveChanges();
                }

            });

            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpGet("/get/groups")]
        public IActionResult GetGroups()
        {
            var groups = new List<GroupVm>();

            context.Groups.Where(x => x.TeacherId == User.GetUserId()).ToList()
                .ForEach(group =>
                {
                    groups.Add(mapper.ToGroupVm(group));
                });
            
            return Ok(groups);
        }

    }
}
