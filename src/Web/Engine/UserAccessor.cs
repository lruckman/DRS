using System.Security.Claims;
using Microsoft.AspNet.Http;

namespace Web.Engine
{
    public interface IUserAccessor
    {
        ClaimsPrincipal User { get; }
    }

    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _accessor;

        public UserAccessor(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public ClaimsPrincipal User => _accessor.HttpContext.User;
    }
}