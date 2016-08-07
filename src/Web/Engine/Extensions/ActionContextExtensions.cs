using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Engine.Extensions
{
    public static class ActionContextExtensions
    {
        /// <summary>
        /// Returns TRUE if the current request is an AJAX request
        /// </summary>
        public static bool IsAjaxRequest(this ActionContext context)
        {
            return context.HttpContext.Request
                .GetTypedHeaders()
                .Accept
                .Any(header => header.MediaType == "application/json");
        }
    }
}