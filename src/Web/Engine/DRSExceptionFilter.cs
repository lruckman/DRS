using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;

namespace Web.Engine
{
    public class DRSExceptionFilter : ActionFilterAttribute, IExceptionFilter, IAsyncExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            //todo: logging

            if (!IsAjaxRequest(context))
            {
                return;
            }

            ReturnExceptionAsJson(context);
        }

        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task OnExceptionAsync(ExceptionContext context)
        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            //todo: logging

            if (!IsAjaxRequest(context))
            {
                return;
            }
            
            ReturnExceptionAsJson(context);
        }

        private static bool IsAjaxRequest(ActionContext context)
        {
            return context.HttpContext.Request
                .GetTypedHeaders()
                .Accept
                .Any(header => header.MediaType == "application/json");
        }

        private static void ReturnExceptionAsJson(ExceptionContext context)
        {
            var jsonResult = new JsonResult(new
            {
                code = 500,
                context.Exception
            });

            jsonResult.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.Result = jsonResult;
        }
    }
}