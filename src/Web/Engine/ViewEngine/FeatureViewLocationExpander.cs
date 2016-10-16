﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Web.Engine.ViewEngine
{
    public class FeatureViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (viewLocations == null)
            {
                throw new ArgumentNullException(nameof(viewLocations));
            }

            var controllerActionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
            {
                throw new NullReferenceException("ControllerActionDescriptor cannot be null.");
            }

            var featureName = controllerActionDescriptor.Properties["feature"] as string;
            return viewLocations.Select(location => location.Replace("{3}", featureName));
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}