using Data.Mapper;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Identity.Attribute;
using Data.Enum;
using Data.VModels;
using Data.DbModels;
using Microsoft.EntityFrameworkCore;

namespace HlasiciSystem.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IApplicationMapper mapper;
        private readonly AppDbContext context;

        public UserController(IApplicationMapper mapper, AppDbContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpGet("/get/users")]
        public IActionResult GetUsers()
        {
            var users = new List<UserVm>();
            context.Users.ToList()
                .ForEach(user => users.Add(mapper.ToUserVm(user)));

            return Ok(users);
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpGet("/get/users/question/{groupId}")]
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
        [HttpGet("/has/question/{groupId}")]
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
