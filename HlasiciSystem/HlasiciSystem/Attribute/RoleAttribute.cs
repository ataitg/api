using HlasiciSystem.Controllers;
using Data.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Data;

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
            var dbContext = (AppDbContext)context.HttpContext.RequestServices.GetService(typeof(AppDbContext));

            var userRole = context.HttpContext.User.GetUserRole(dbContext);

            if((userRole & role) != role)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
