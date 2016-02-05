using System.IO;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using StructureMap;
using StructureMap.Graph;
using Web.Engine.Services.Lucene;

namespace Web.Engine
{
    public class WebRegistry : Registry
    {
        public WebRegistry()
        {
            Scan(scanner =>
            {
                scanner.Assembly(Assembly.GetExecutingAssembly());
                scanner.TheCallingAssembly();
                scanner.WithDefaultConventions();

                // AutoMapper

                scanner.AddAllTypesOf<Profile>();

                // MediatR

                scanner.AssemblyContainingType<IMediator>();
                scanner.ConnectImplementationsToTypesClosing(typeof (IRequestHandler<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof (IAsyncRequestHandler<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof (INotificationHandler<>));
                scanner.ConnectImplementationsToTypesClosing(typeof (IAsyncNotificationHandler<>));
            });

            // MediatR

            For<SingleInstanceFactory>()
                .Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>()
                .Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            // Lucene

            For<IIndexer>()
                .Use(() => new Indexer(Startup.Configuration["DRS.IndexDirectory"]));

            // AutoMapper

            For<IMappingEngine>().Use(() => Mapper.Engine);

            // Fluent Validation

            For<IModelValidatorProvider>()
                .Use<FluentValidationModelValidatorProvider>();
            For<IValidatorFactory>()
                .Use<StructureMapValidatorFactory>();
        }
    }
}