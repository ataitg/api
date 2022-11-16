using HlasiciSystem.Controllers;
using Data.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Identity.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RoleAttribute : System.Attribute, IAuthorizationFilter
    {
        private UserRoles role;
        public RoleAttribute(UserRoles role)
        {
            this.role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userRole = context.HttpContext.User.GetUserRole();

            if((userRole & role) != role)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
