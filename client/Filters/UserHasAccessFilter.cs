using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace client.Filters
{
    public class UserHasAccessFilter : ActionFilterAttribute
    {
        internal string RolPermitido { get; }

        public UserHasAccessFilter(string rol)
        {
            RolPermitido = rol;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? roles = context.HttpContext.Session.GetString("loggedUserRoles");

            if (roles == null || !roles.Contains(RolPermitido))
            {
                context.Result = new RedirectResult("~/usuario/notauthorized");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}