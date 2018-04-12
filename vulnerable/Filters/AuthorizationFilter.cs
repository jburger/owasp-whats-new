using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using vulnerable.Domain;

namespace vulnerable.Filters 
{
    public class AuthorizationFilter : ActionFilterAttribute 
    {
        public override void OnResultExecuting(ResultExecutingContext context) 
        {
            ((Controller)context.Controller).ViewBag.User = Utils.GetIdentity(context.HttpContext.Request);
            ((Controller)context.Controller).ViewBag.IsUser = Utils.IsUser(context.HttpContext.Request);
            ((Controller)context.Controller).ViewBag.IsAdmin = Utils.IsAdmin(context.HttpContext.Request);
        }
    }
}