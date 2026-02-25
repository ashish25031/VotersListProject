using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VotersListProject.Models
{
    public class SetSessionGlobally : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var result = context.HttpContext.Session.GetString("UserName");
            if (result == null)
            {
                context.Result = new RedirectToActionResult("Login", "Voters", null);
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
