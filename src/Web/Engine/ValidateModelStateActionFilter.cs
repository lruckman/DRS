using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;

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
                filterContext.Result = controller.HttpBadRequest();
            }
            else
            {
                filterContext.Result = controller.HttpBadRequest(controller.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}