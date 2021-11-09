using Azure.Core.Serialization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace functionsapp.Helpers
{
    public static class DynamicHelpers
    {
        public static object ConvertToTypedList(IList<object> values, Type type)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);

            var instance = Activator.CreateInstance(constructedListType);
            var methodListAdd = constructedListType.GetMethod("Add");
            foreach (var value in values)
            {
                methodListAdd.Invoke(instance, new object[] { value });
            }
            return instance;
        }

        public static IList<object> GetConvertersWithoutConstructor(Assembly assembly)
        {
            var converterTypes = assembly.GetTypes().Where(type => type.FullName.StartsWith("Microsoft.Azure.Functions.Worker.Converters") && type.IsClass && type.Name.EndsWith("Converter")
            && type.Name != "JsonPocoConverter");
            return converterTypes.Select(t => Activator.CreateInstance(t)).ToList();
        }

        public static object GetJsonProco(Assembly assembly)
        {
            var jsonProco = assembly.GetTypes().Where(type => type.Name == "JsonPocoConverter").FirstOrDefault();
            ObjectSerializer serializer = new JsonObjectSerializer();

            var workerOptionsType = assembly.GetTypes().Where(type => type.Name == "WorkerOptions").FirstOrDefault();
            var workerOptions = Activator.CreateInstance(workerOptionsType);
            var property = workerOptionsType.GetProperty("Serializer");
            property.SetValue(workerOptions, serializer);
            var optionsType = typeof(Options);
            var methodCreateOptions = optionsType.GetMethod("Create").MakeGenericMethod(workerOptionsType);
            var options = methodCreateOptions.Invoke(null, new object[] { workerOptions });

            return Activator.CreateInstance(jsonProco, new object[] { options });
        }
        public static object GetConverters(Assembly assembly)
        {
            var type = assembly.DefinedTypes.Where(t => t.Name.Contains("IConverter")).FirstOrDefault();
            var converters = GetConvertersWithoutConstructor(assembly);
            converters.Add(GetJsonProco(assembly));
            return ConvertToTypedList(converters, type);

        }

        public static object GetDefaultModelBindingFeature(Assembly assembly)
        {
            var converterObjects = GetConverters(assembly);

            var defaultModelBindingFeatureType = assembly.GetTypes().Where(t => t.Name == "DefaultModelBindingFeature").FirstOrDefault();

            return Activator.CreateInstance(defaultModelBindingFeatureType, new object[] { converterObjects });
        }
    }
}
