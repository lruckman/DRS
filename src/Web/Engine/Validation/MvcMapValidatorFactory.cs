using System;
using FluentValidation;

namespace Web.Engine
{
    public class MvcMapValidatorFactory : ValidatorFactoryBase
    {
        private readonly IServiceProvider _serviceProvider;

        public MvcMapValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return _serviceProvider.GetService(validatorType) as IValidator;
        }
    }
}