using Data.Mapper;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Identity.Attribute;
using Data.Enum;
using Data.VModels;
using Data.DbModels;

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
    }
}
