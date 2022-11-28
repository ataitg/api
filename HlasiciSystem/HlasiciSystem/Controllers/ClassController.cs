using Data;
using Data.APIModels;
using Data.DbModels;
using Data.Mapper;
using Data.VModels;
using Identity.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Data.Enum;

namespace HlasiciSystem.Controllers
{
    [ApiController]
    [Route("api/classes")]
    public class ClassController : ControllerBase
    {
        private readonly IApplicationMapper mapper;
        private readonly AppDbContext context;
        public ClassController(IApplicationMapper mapper, AppDbContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpPost]
        public IActionResult CreateClass([FromBody] CreateClass model)
        {
            var classs = mapper.ToClass(model); 
            context.Classes.Add(classs);
            context.SaveChanges();
            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpDelete("{classId}")]
        public IActionResult DeleteClass([FromRoute] string classId)
        {
            var classs = context.Classes.FirstOrDefault(x => x.Id.ToString() == classId);
            if (classs == null)
            {
                return BadRequest();
            }
            context.Classes.Remove(classs);
            context.SaveChanges();
            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpGet]
        public IActionResult GetClasses()
        {
            var classes = new List<ClassVm>();
            context.Classes.ToList().ForEach(classs =>
            {
                classes.Add(mapper.ToClassVm(classs));
            });
            return Ok(classes);
        }


        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpGet("{classId}/users")]
        public IActionResult GetClassUsers([FromRoute] string classId)
        {
            var classs = context.Classes.FirstOrDefault(x => x.Id.ToString() == classId);
            if (classs == null)
            {
                return BadRequest();
            }
            var users = new List<UserVm>();
            context.UserClasses.Where(x => x.ClassId == classs.Id).Include(y => y.User).ToList()
                .ForEach(UserClass =>
                {
                    users.Add(mapper.ToUserVm(UserClass.User));
                });
            return Ok(users);
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpPost("{classId}/users")]
        public IActionResult AddUsersToClass([FromRoute] string classId, [FromBody] Users model)
        {
            var classs = context.Classes.FirstOrDefault(x => x.Id.ToString() == classId);
            if(classs == null)
            {
                return BadRequest();
            }
            model.Ids.ForEach(userId =>
            {
                var user = context.Users.FirstOrDefault(x => x.Id.ToString() == userId);
                if(user != null)
                {
                    context.UserClasses.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        ClassId = classs.Id,
                        UserId = user.Id
                    });
                    context.SaveChanges();
                }
            });
            return Ok();
        }


        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpDelete("{classId}/users")]
        public IActionResult RemoveUsersFromClass([FromRoute] string classId, [FromBody] Users model)
        {
            var classs = context.Classes.FirstOrDefault(x => x.Id.ToString() == classId);
            if(classs == null)
            {
                return BadRequest();
            }
            model.Ids.ForEach(userId =>
            {
                var userClass = context.UserClasses.FirstOrDefault(x => x.UserId.ToString() == userId && x.ClassId == classs.Id);
                if(userClass != null)
                {
                    context.UserClasses.Remove(userClass);
                    context.SaveChanges();
                }
            });
            return Ok();
        }

        [Authorize]
        [Role(UserRoles.Teacher)]
        [HttpPatch("{classId}")]
        public IActionResult RenameClass([FromBody] string newName, [FromRoute] string classId)
        {
            var classs = context.Classes.FirstOrDefault(x => x.Id.ToString() == classId);
            if (classs == null)
            {
                return BadRequest();
            }

            classs.Name = newName;
            context.Classes.Update(classs);
            context.SaveChanges();

            return Ok();
        }
    }
}
