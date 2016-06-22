using System;
using Microsoft.AspNetCore.Mvc;

namespace WebTests.Controllers
{
    public class BaseControllerTest<TController> where TController : Controller
    {
        protected static readonly byte[] TestFile1Bytes =
        {
            0x1E
        };

        protected const string TestFile1ContentType = "application/text";

        /// <summary>
        /// Creates a controller instance for testing
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Controller of type <typeparamref name="TController"/></returns>
        protected TController CreateController(params object[] parameters)
        {
            return (TController)Activator.CreateInstance(typeof(TController), parameters);
        }
    }
}