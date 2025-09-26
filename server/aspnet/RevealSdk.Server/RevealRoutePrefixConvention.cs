using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace RevealSdk.Server
{
    public static class RevealRoutePrefixConvention
    {
        public static void AddRevealRoutePrefix(this MvcOptions options, string prefix)
        {
            options.Conventions.Add(new ChangeRevealPrefixConvention(prefix));
        }
    }

    /// <summary>
    /// A convention to change the route prefix for Reveal controllers
    /// </summary>
    public class ChangeRevealPrefixConvention : IControllerModelConvention
    {
        private readonly string _routePrefix;        

        public ChangeRevealPrefixConvention(string prefix)
        {
            _routePrefix = prefix;
        }

        public void Apply(ControllerModel controller)
        {
            // Only apply to controllers in the Embedalytics.SDK namespace
            var controllerNamespace = controller.ControllerType.Namespace;            
            if (controllerNamespace != null && controllerNamespace.StartsWith("Infragistics.EMServer.Web.Controllers.WebAPI"))
            {
                
                // Get the existing attribute routes
                var attributeRoutes = controller.Selectors
                    .Where(s => s.AttributeRouteModel != null)
                    .Select(s => s.AttributeRouteModel)
                    .ToList();

                if (attributeRoutes.Any())
                {
                    // Replace each route template with the prefixed version
                    foreach (var route in attributeRoutes)
                    {
                        if (route.Template != null)
                        {
                            route.Template = $"{_routePrefix}/{route.Template.TrimStart('/')}".TrimEnd('/');
                        }
                    }
                }

                if (controller.ControllerName == "SdkTools")
                {
                    foreach (var action in controller.Actions)
                    {
                        foreach (var selector in action.Selectors)
                        {
                            GenerateTemplate(selector);
                        }
                    }
                }
            }

        }

        private void GenerateTemplate(SelectorModel selector)
        {
            if (selector.AttributeRouteModel?.Template != null)
            {
                var existingTemplate = selector.AttributeRouteModel.Template;
                var newTemplate = $"{_routePrefix}/{existingTemplate.TrimStart('/')}";
                selector.AttributeRouteModel.Template = newTemplate;
            }
        }
    }
}
