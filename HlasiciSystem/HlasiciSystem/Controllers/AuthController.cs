using Data;
using Data.DbModels;
using HlasiciSystem.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HlasiciSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Mail);

            if (user == null)
            {
                return BadRequest();
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            var userPrincipal = await signInManager.CreateUserPrincipalAsync(user);
            await HttpContext.SignInAsync(userPrincipal);

            return NoContent();
        }

        [Authorize]
        [HttpGet("/logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return NoContent();
        }

    }
    public class LoginModel
    {
        public string Mail { get; set; }
        public string Password { get; set; }
    }

    public static class AuthExtensions
    {
        private static AppDbContext dbContext;

        public static void Configure(AppDbContext context)
        {
            dbContext = context;
        }

        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException("user not logged in");
            }
            var idString = user.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return Guid.Parse(idString);
        }

        public static UserRoles GetUserRole(this ClaimsPrincipal user)
        {
            var account = dbContext.Users.FirstOrDefault(x => x.Id == user.GetUserId());
            if (account == null)
            {
                return UserRoles.None;
            }
            return (UserRoles)account.Role;
        }
    }
}
