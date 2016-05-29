using System;
using System.Linq;
using AutoMapper;
using StructureMap;

namespace Web.Engine.Mapping
{
    public class AutoMapperRegistry : Registry
    {
        public AutoMapperRegistry()
        {
            var profiles =
                typeof (WebRegistry).Assembly.GetTypes()
                    .Where(t => typeof (Profile).IsAssignableFrom(t))
                    .Select(t => (Profile) Activator.CreateInstance(t));

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            config.AssertConfigurationIsValid(); // ¯\_(ツ)_/¯

            For<MapperConfiguration>()
                .Singleton()
                .Use(config);

            For<IConfigurationProvider>()
                .Singleton()
                .Use(config);

            For<IMapper>()
                .Singleton()
                .Use(ctx => ctx.GetInstance<MapperConfiguration>()
                    .CreateMapper(ctx.GetInstance));
        }
    }
}