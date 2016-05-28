using System.Collections.Generic;

namespace Web.ViewModels.Api
{
    public class ApiError
    {
        public IDictionary<string, IEnumerable<string>> Errors { get; set; }
    }
}