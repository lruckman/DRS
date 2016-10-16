using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Engine.Extensions;

namespace Web.Engine.Filters
{
    public class ValidateModelStateFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (Controller) filterContext.Controller;

            if (controller.ViewData.ModelState.IsValid)
            {
                return;
            }

            if (!filterContext.IsAjaxRequest())
            {
                filterContext.Result = controller.BadRequest();
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