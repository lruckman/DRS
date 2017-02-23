using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Web.Engine.Services.Hangfire.Jobs;

namespace Web.Engine.Services.Hangfire
{
    public static class HangfireConfiguration
    {
        public static void Configure(this IApplicationBuilder app, IServiceProvider container, string connectionString)
        {
            GlobalConfiguration.Configuration.UseActivator(new StructureMapJobActivator(container));

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

            RecurringJob
                .AddOrUpdate<IndexRevisionsJob>(nameof(IndexRevisionsJob)
                , j => j.Run()
                , "*/5 * * * *", TimeZoneInfo.Local);
        }

        public static void ConfigureServices(this IServiceCollection services, string connectionString)
        {
            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
        }

        public class StructureMapJobActivator : JobActivator
        {
            private readonly IServiceProvider container;

            public StructureMapJobActivator(IServiceProvider container)
            {
                this.container = container ?? throw new ArgumentNullException(nameof(container));
            }

            public override object ActivateJob(Type jobType)
            {
                return container.GetService(jobType);
            }
        }
    }
}
