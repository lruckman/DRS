using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Engine.Extensions;
using Web.ViewModels.Api;

namespace Web.Engine
{
    public class ValidateModelStateActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (Controller) filterContext.Controller;

            if (controller.ViewData.ModelState.IsValid)
            {
                return;
            }

            filterContext.Result =
                controller.BadRequest(new ApiError {Errors = controller.ModelState.ToSimpleDictionary()});
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}