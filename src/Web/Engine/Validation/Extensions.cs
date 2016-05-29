﻿using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Engine.Validation
{
    public static class Extensions
    {
        /// <summary>
        ///     Adds Fluent Validation services to the specified
        ///     <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" />.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" /> that can be used to further configure the
        ///     MVC services.
        /// </returns>
        public static IMvcBuilder FluentValidation(this IMvcBuilder mvcBuilder)
        {
            // add all IValidator to MVC's service provider

            var validators =
                typeof (FluentValidationObjectModelValidatorProvider)
                    .Assembly
                    .GetTypes()
                    .Where(t => typeof (IValidator).IsAssignableFrom(t));

            foreach (var validator in validators)
            {
                mvcBuilder.Services.AddTransient(validator);
            }

            // add the fluent validation object model validator

            mvcBuilder.Services.Add(ServiceDescriptor.Transient<IObjectModelValidator>(serviceProvider =>
                new FluentValidationObjectModelValidatorProvider(
                    serviceProvider.GetRequiredService<IModelMetadataProvider>(),
                    new MvcMapValidatorFactory(serviceProvider.GetRequiredService<IServiceProvider>()))));

            // clear all model validation providers since fluent validation will be handling everything

            mvcBuilder.AddMvcOptions(
                options => { options.ModelValidatorProviders.Clear(); });

            return mvcBuilder;
        }
    }
}