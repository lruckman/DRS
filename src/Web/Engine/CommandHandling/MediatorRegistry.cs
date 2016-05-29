using MediatR;
using StructureMap;

namespace Web.Engine.CommandHandling
{
    public class MediatorRegistry : Registry
    {
        public MediatorRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssemblyContainingType<IMediator>();
                scanner.AssemblyContainingType<MediatorRegistry>();
                scanner.WithDefaultConventions();

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
        }
    }
}