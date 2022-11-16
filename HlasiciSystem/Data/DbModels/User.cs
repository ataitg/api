using Data.Enum;
using Data.Mapper;
using Data.VModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DbModels
{
    public class User : IdentityUser<Guid>
    {
        public UserRoles Role { get; set; }
    }

    public static class UserExtensions
    {
        public static UserVm ToUserVm(this IApplicationMapper mapper, User user)
        {
            return new()
            {
                Id = user.Id.ToString(),
                UserName = user.UserName,
            };
        }
    }
}
