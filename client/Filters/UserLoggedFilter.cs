using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace client.Filters
{
    public class UserLoggedFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? roles = context.HttpContext.Session.GetString("loggedUser");

            if (roles == null)
            {
                context.Result = new RedirectResult("~/usuario/login");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}