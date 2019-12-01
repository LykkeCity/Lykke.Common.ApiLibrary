using System;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Common.ApiLibrary.Swagger.XmsEnum
{
    /// <summary>
    /// Swagger schema filter, which applies x-ms-enum extension to the enum properties of the model schema
    /// </summary>
    [PublicAPI]
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

            if (!(context.JsonContract is JsonObjectContract jsonContract))
                return;

            if (jsonContract == null)
            {
                throw new InvalidOperationException($"JSON contract is not defined for type {context.SystemType}");
            }

            foreach (var property in model.Properties.Where(x => x.Value.Enum != null))
            {
                var jsonProperty = jsonContract.Properties?.GetProperty(property.Key, StringComparison.Ordinal);

                if (jsonProperty == null)
                {
                    throw new InvalidOperationException($"Property {property.Key} not found in JSON contract for type {jsonContract.UnderlyingType}");
                }

                XmsEnumExtensionApplicator.Apply(property.Value.Extensions, jsonProperty.PropertyType, _options);
            }
        }
    }
}
