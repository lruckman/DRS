using Hangfire;
using System;

namespace Web.Engine.Services.Hangfire
{
    public class HangfireActivator : JobActivator
    {
        private readonly IServiceProvider _container;

        public HangfireActivator(IServiceProvider container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public override object ActivateJob(Type jobType)
        {
            return _container.GetService(jobType);
        }
    }
}
