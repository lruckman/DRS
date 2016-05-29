using System;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web.Engine
{
    //TODO: Need support for CustomizeValidatorAttribute and client-side

    public class FluentValidationObjectModelValidatorProvider : IObjectModelValidator
    {
        private readonly IValidatorFactory _validatorFactory;

        /// <summary>
        ///     Initializes a new instance of <see cref="FluentValidationObjectModelValidatorProvider" />.
        /// </summary>
        public FluentValidationObjectModelValidatorProvider(IModelMetadataProvider modelMetadataProvider,
            IValidatorFactory validatorFactory)
        {
            if (modelMetadataProvider == null)
            {
                throw new ArgumentNullException(nameof(modelMetadataProvider));
            }

            _validatorFactory = validatorFactory;
        }

        public void Validate(ActionContext actionContext, IModelValidatorProvider validatorProvider,
            ValidationStateDictionary validationState, string prefix, object model)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (model == null)
            {
                return;
            }
            
            var validator = _validatorFactory.GetValidator(model.GetType());

            if (validator == null)
            {
                return;
            }

            var result = validator.Validate(model);

            foreach (var modelError in result.Errors)
            {
                actionContext.ModelState.AddModelError(modelError.PropertyName, modelError.ErrorMessage);
            }
        }
    }
}