using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Engine
{
    public class ValidateModelStateActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (Controller)filterContext.Controller;

            if (controller.ViewData.ModelState.IsValid)
            {
                return;
            }

            if (filterContext.HttpContext.Request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
            {
                filterContext.Result = controller.BadRequest();
            }
            else
            {
                filterContext.Result = controller.BadRequest(controller.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}