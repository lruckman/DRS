using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Engine.Services.Hangfire
{
    public static class HangfireConfiguration
    {
        public static void Configure(this IApplicationBuilder app, string connectionString)
        {
            app.UseHangfireServer(
                options: new BackgroundJobServerOptions
                {
                    Activator = new JobActivator()
                },
                storage: new SqlServerStorage(connectionString));

            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new IDashboardAuthorizationFilter[] { new DashboardAuthorizationFilter() }
            });
        }

        public static void ConfigureServices(this IServiceCollection services, string connectionString)
        {
            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
        }
    }
}
