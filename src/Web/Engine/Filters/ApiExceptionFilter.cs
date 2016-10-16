using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Engine.Extensions;

namespace Web.Engine.Filters
{
    public class ApiExceptionFilter : ActionFilterAttribute, IExceptionFilter, IAsyncExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            HandleException(context);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task OnExceptionAsync(ExceptionContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            HandleException(context);
        }

        private static void HandleException(ExceptionContext context)
        {
            //todo: logging

            if (!context.IsAjaxRequest())
            {
                return;
            }

            var data = new ApiError
            {
                Errors = new Dictionary<string, IEnumerable<string>>
                {
                    {"*", new[] {context.Exception.Message}}
                }
            };

            var jsonResult = new JsonResult(data) {StatusCode = (int) HttpStatusCode.InternalServerError};

            context.Result = jsonResult;
        }
    }
}