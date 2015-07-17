using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignMeUp2.ViewModels
{
    public class WizardModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            if (modelType == typeof(WizardStep))
            {
                //var stepTypeValue = bindingContext.ValueProvider.GetValue("StepType");
                //var stepType = Type.GetType((string)stepTypeValue.ConvertTo(typeof(string)), true);
                //var step = Activator.CreateInstance(stepType);
                //bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => step, stepType);

                HttpRequestBase request = controllerContext.HttpContext.Request;

                var faltLista = new List<FaltViewModel>();

                for (int i = 0; i < request.Form.Count; i++)
                {
                    if (request.Form.Keys.Get(i).Substring(0, 5) == "falt_")
                    {
                        faltLista.Add(new FaltViewModel { Varde = request.Form.Get(i) });
                    }
                }

                var wizardStep = new WizardStep
                {
                    Namn = request.Form.Get("Namn"),
                    StepCount = int.Parse(request.Form.Get("StepCount")),
                    StepIndex = int.Parse(request.Form.Get("StepIndex")),
                    FaltLista = faltLista
                };

                return wizardStep;
            }
            return base.CreateModel(controllerContext, bindingContext, modelType);
        }
    }
}