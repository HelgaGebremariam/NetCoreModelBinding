using functionsapp.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace functionsapp.Middleware
{
    public class ModelBindingMiddleware : IFunctionsWorkerMiddleware
    {
        private static void AddModelBindingFeature(FunctionContext context)
        {
            var assembly = Assembly.GetAssembly(typeof(IFunctionsWorkerMiddleware));
            var defaultModelBindingFeature = DynamicHelpers.GetDefaultModelBindingFeature(assembly);

            var modelBindingFeatureType = assembly.GetTypes().Where(t => t.Name == "IModelBindingFeature").FirstOrDefault();
            var method = context.Features.GetType().GetMethod("Set").MakeGenericMethod(modelBindingFeatureType);
            method.Invoke(context.Features, new object[] { defaultModelBindingFeature });
        }

        public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            AddModelBindingFeature(context);

            return next(context);
        }
    }
}
