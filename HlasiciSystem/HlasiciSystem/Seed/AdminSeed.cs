using Data;
using Microsoft.AspNetCore.Identity;

namespace HlasiciSystem.Seed
{
    public class AdminSeed
    {
        public static async Task CreateAdminAsync(UserManager<User> userManager)
        {
            var admin = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(admin.Email);

            if (user == null)
            {
                var created = await userManager.CreateAsync(admin, "Heslo#50");

                if (!created.Succeeded)
                {
                    throw new Exception($"Something went wrong in creating of user.\n{string.Join("\n", created.Errors.Select(x => x.Description))}");
                }
            }
        }
    }
}
