using Data;
using Data.APIModels;
using Data.DbModels;
using Data.Mapper;
using Data.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost("/create/class")]
        public IActionResult CreateClass([FromBody] CreateClass model)
        {
            var classs = mapper.ToClass(model);
            context.Classes.Add(classs);
            context.SaveChanges();
            return Ok();
        }

        [HttpDelete("/delete/class/{classId}")]
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

        [HttpGet("/get/classes")]
        public IActionResult GetClasses()
        {
            var classes = new List<ClassVm>();
            context.Classes.ToList().ForEach(classs =>
            {
                classes.Add(mapper.ToClassVm(classs));
            });
            return Ok(classes);
        }

        [HttpGet("get/users/class/{classId}")]
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
    }
}
