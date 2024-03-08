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
    public class ChangeRevealPrefixConvention : IApplicationModelConvention
    {
        private readonly string _prefix;

        public ChangeRevealPrefixConvention(string prefix)
        {
            _prefix = prefix;
        }

        public void Apply(ApplicationModel application)
        {
            //todo: customize to fit your needs
            foreach (var controller in application.Controllers)
            {
                if (controller.ControllerName == "DashboardFile" || controller.ControllerName == "SdkReveal")
                {
                    foreach (var selector in controller.Selectors)
                    {
                        GenerateTemplate(selector);
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
            var existingTemplate = selector.AttributeRouteModel.Template ?? string.Empty;
            var newTemplate = $"{_prefix}/{existingTemplate.TrimStart('/')}";
            selector.AttributeRouteModel.Template = newTemplate;
        }
    }
}
