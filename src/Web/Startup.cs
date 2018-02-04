using AutoMapper;
using FluentValidation.AspNetCore;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Serilog;
using StructureMap;
using System;
using Web.Engine;
using Web.Engine.Filters;
using Web.Engine.Services;
using Web.Engine.Services.Hangfire;
using Web.Engine.Services.Hangfire.Jobs;
using Web.Models;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .Enrich.FromLogContext()
            //    .WriteTo.LiterateConsole()
            //    .WriteTo.Async(a => a.RollingFile(Path.Combine(env.ContentRootPath, "logs/log-{Date}.txt")))
            //    .CreateLogger();

            Configuration = configuration;
        }

        private static IConfiguration Configuration { get; set; }
        private static string DefaultConnectionString => Configuration.GetConnectionString("DefaultConnection");

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(DefaultConnectionString, o => o.EnableRetryOnFailure()));

            services.Configure<DRSConfig>(Configuration.GetSection("DRS"));
            services.Configure<Engine.Services.Lucene.Config>(Configuration.GetSection("DRS"));
            services.Configure<OcrEngine.Config>(Configuration.GetSection("DRS:Tess"));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication();

            services.AddHangfire(x => x.UseSqlServerStorage(DefaultConnectionString));

            services
                .AddMvc(
                    options =>
                    {
                        options.Filters.Add(new ValidateModelStateFilter());
                        options.Filters.Add(new ApiExceptionFilter());
                    })
                .AddFeatureFolders()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddAutoMapper(typeof(Startup));

            Mapper.AssertConfigurationIsValid();

            services.AddMediatR(typeof(Startup));

            var container = new Container(cfg => cfg.AddRegistry<WebRegistry>());

            // populates structuremap with .NET services

            container.Populate(services);

            FileDecoder.RegisterFileDecoders(container.GetInstance<IOcrEngine>());

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime,
            IServiceProvider serviceProvider)
        {
            loggerFactory.AddSerilog();
            // Ensure any buffered events are sent at shutdown
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();

                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    HotModuleReplacement = true,
                //    ReactHotModuleReplacement = true
                //});
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            // Hangfire

            GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(serviceProvider));

            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new DashboardAuthorizationFilter() }
            });

            RecurringJob
                .AddOrUpdate<IndexRevisions>(nameof(IndexRevisions)
                , j => j.Run()
                , "*/5 * * * *", TimeZoneInfo.Local);

            RecurringJob
                .AddOrUpdate<GenerateThumbnails>(nameof(GenerateThumbnails)
                , j => j.Run()
                , "*/5 * * * *", TimeZoneInfo.Local);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}");

                //routes.MapSpaFallbackRoute(
                //    "spa-fallback",
                //    "{controller=Documents}/{action=Index}");
            });
        }
    }
}