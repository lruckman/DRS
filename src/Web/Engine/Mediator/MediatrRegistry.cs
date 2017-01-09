using MediatR;
using StructureMap;
using Web.Engine.Mediator.Behaviors;

namespace Web.Engine.Mediator
{
    public class MediatrRegistry : Registry
    {
        public MediatrRegistry()
        {
            // register in the order you want them to run in
            For(typeof(IPipelineBehavior<,>)).Add(typeof(LoggingBehavior<,>));
        }
    }
}
