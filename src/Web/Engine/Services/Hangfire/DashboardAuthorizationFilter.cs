using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Web.Engine.Services.Hangfire
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true; //todo: don't leave up to everyone
        }
    }
}
