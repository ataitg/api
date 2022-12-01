using Data;
using Data.APIModels;
using Data.DbModels;
using Data.Mapper;
using Data.Enum;
using Identity.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Data.VModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace HlasiciSystem.Controllers
{
    [ApiController]
    [Route("api/groups")]
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
        [HttpPost]
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
        [HttpDelete("{groupId}")]
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
        [HttpGet("{groupId}/activate")]
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
        [HttpGet("{groupId}/deactivate")]
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

            context.UserGroups.Where(x => x.GroupId == group.Id).ToList()
                .ForEach(userGroup => context.UserGroups.Remove(userGroup));

            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpPatch("{groupId}")]
        public IActionResult RenameGroup([FromRoute] string groupId, [FromBody] JsonPatchDocument<UpdateGroup> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var group = context.Groups.FirstOrDefault(x => x.Id.ToString() == groupId);
            if (group == null)
            {
                return BadRequest();
            }

            if (group.TeacherId != User.GetUserId())
            {
                return Forbid();
            }

            var updateModel = mapper.ToUpdateGroup(group);

            patchDoc.ApplyTo(updateModel, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var updatedGroup = mapper.ToGroup(updateModel, group);

            context.Groups.Update(updatedGroup);
            context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpPost("{groupId}/users")]
        public IActionResult AddUsersToGroup([FromRoute] string groupId, [FromBody] Users model)
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

            model.Ids.ForEach(userId =>
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
        [HttpDelete("{groupId}/remove")]
        public IActionResult RemoveUsersFromGroup([FromRoute] string groupId, [FromBody] Users model)
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

            model.Ids.ForEach(userId =>
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
        [HttpGet]
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

        [Authorize]
        [Role(UserRoles.User)]
        [HttpGet("/student")]
        public IActionResult GetGroupsAsStudent()
        {
            var groups = new List<GroupVm>();
            context.UserGroups.Where(x => x.UserId == User.GetUserId()).Include(y => y.Group).ToList()
                .ForEach(group =>
                {
                    groups.Add(mapper.ToGroupVm(group.Group));
                });

            return Ok(groups);
        }

        [HttpGet("{groupId}/users")]
        public IActionResult GetGroupUsers([FromRoute] string groupId)
        {
            var group = context.Groups.FirstOrDefault(x => x.Id.ToString() == groupId);
            if(group == null)
            {
                return BadRequest();
            }
            var users = new List<UserVm>();
            context.UserGroups.Where(x => x.GroupId == group.Id).Include(y => y.User).ToList()
                .ForEach(UserGroup =>
                {
                    users.Add(mapper.ToUserVm(UserGroup.User));
                });
            return Ok(users);
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpGet("{groupId}/users/inquiry")]
        public IActionResult GetUsersWithQuestion([FromRoute] string groupId)
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

            var users = new List<UserVm>();
            context.UserGroups.Where(x => x.Id == group.Id && x.HasQuestion).Include(y => y.User).ToList()
                .ForEach(userGroup => users.Add(mapper.ToUserVm(userGroup.User)));

            return Ok(users);
        }

        [Authorize]
        [Role(UserRoles.User)]
        [HttpGet("{groupId}/inquiry")]
        public IActionResult HasQuestion([FromRoute] string groupId)
        {
            var group = context.Groups.FirstOrDefault(x => x.Id.ToString() == groupId);
            if (group == null)
            {
                return BadRequest();
            }

            var userGroup = context.UserGroups.FirstOrDefault(x => x.UserId == User.GetUserId() && x.GroupId.ToString() == groupId);
            if (userGroup == null)
            {
                return BadRequest();
            }

            userGroup.HasQuestion = !userGroup.HasQuestion;

            context.UserGroups.Update(userGroup);
            context.SaveChanges();

            return Ok();
        }
    }
}
