using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Common.ApiLibrary.Swagger.XmsEnum
{
    public class XmsEnumSchemaFilter : ISchemaFilter
    {
        private readonly XmsEnumExtensionsOptions _options;

        public XmsEnumSchemaFilter(XmsEnumExtensionsOptions options)
        {
            _options = options;
        }

        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (model.Properties == null)
            {
                return;
            }

            var systemProperties = GetPublicProperties(context.SystemType);

            foreach (var property in model.Properties.Where(x => x.Value.Enum != null))
            {
                var systemProperty = systemProperties
                    .SingleOrDefault(p => string.Equals(p.Name, property.Key, StringComparison.OrdinalIgnoreCase));

                if (systemProperty == null)
                {
                    throw new InvalidOperationException($"Property {property.Key} not found in type {context.SystemType}");
                }
                
                XmsEnumExtensionApplicator.Apply(property.Value.Extensions, systemProperty.PropertyType, _options);
            }
        }

        private static PropertyInfo[] GetPublicProperties(Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();
                var considered = new List<Type>();
                var queue = new Queue<Type>();

                considered.Add(type);
                queue.Enqueue(type);

                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface))
                        {
                            continue;
                        }

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}