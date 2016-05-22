using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web.Engine
{
    //TODO: Need support for CustomizeValidatorAttribute and client-side

    public class FluebtValudationObjectValidator : IObjectModelValidator
    {
        public IValidatorFactory ValidatorFactory { get; private set; }

        public void Validate(ActionContext actionContext, IModelValidatorProvider validatorProvider,
            ValidationStateDictionary validationState, string prefix, object model)
        {
            var validator = ValidatorFactory.GetValidator(model.GetType());
            var validationResults = validator.Validate(model);

            foreach (var error in validationResults.Errors)
            {
                actionContext.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }
    }

    //public class FluentValidationModelValidatorProvider : IModelValidatorProvider
    //{
    //    public IValidatorFactory ValidatorFactory { get; private set; }

    //    public void GetValidators(ModelValidatorProviderContext context)
    //    {
    //        IValidator validator = CreateValidator(context);

    //        if (!IsValidatingProperty(context))
    //        {
    //            context.Validators.Add(new FluentValidationModelValidator(validator));
    //        }
    //    }

    //    protected virtual IValidator CreateValidator(ModelValidatorProviderContext context)
    //    {
    //        if (IsValidatingProperty(context))
    //        {
    //            return ValidatorFactory.GetValidator(context.ModelMetadata.ContainerType);
    //        }
    //        return ValidatorFactory.GetValidator(context.ModelMetadata.ModelType);
    //    }

    //    protected virtual bool IsValidatingProperty(ModelValidatorProviderContext context)
    //    {
    //        return context.ModelMetadata.ContainerType != null && !string.IsNullOrEmpty(context.ModelMetadata.PropertyName);
    //    }

    //    public void CreateValidators(ModelValidatorProviderContext context)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}

    public class FluentValidationModelValidator : IModelValidator
    {
        private readonly IValidator _validator;

        public FluentValidationModelValidator(IValidator validator)
        {
            _validator = validator;
        }

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            var model = context.Model;

            var result = _validator.Validate(model);

            return from error in result.Errors
                   select new ModelValidationResult(error.PropertyName, error.ErrorMessage);

        }

        public bool IsRequired => false;
    }
}