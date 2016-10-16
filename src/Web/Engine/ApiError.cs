using System.Collections.Generic;

namespace Web.Engine
{
    public class ApiError
    {
        public IDictionary<string, IEnumerable<string>> Errors { get; set; }
    }
}