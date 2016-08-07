using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Engine.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        /// <summary>
        /// Returns the errors contained in the ModelState as a Dictionary object.
        /// </summary>
        public static IDictionary<string, IEnumerable<string>> ToSimpleDictionary(
            this ModelStateDictionary modelStateDictionary)
        {
            return modelStateDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage));
        }
    }
}