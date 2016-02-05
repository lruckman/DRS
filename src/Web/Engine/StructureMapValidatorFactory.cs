using System;
using FluentValidation;

namespace Web.Engine
{
    public class StructureMapValidatorFactory : ValidatorFactoryBase
    {
        public override IValidator CreateInstance(Type validatorType)
        {
            return Startup.IoContainer.TryGetInstance(validatorType) as IValidator;
        }
    }
}