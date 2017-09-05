using System;
using System.Linq;
using System.Reflection;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

namespace Lykke.Common.ApiLibrary.Swagger.XmsEnum
{
    public class ResponseValueTypesRequiredSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            if (schema.Type != "object" || schema.Properties == null)
            {
                return;
            }

            var nonNulableValueTypedPropNames = context.SystemType.GetProperties()
                .Where(p => p.PropertyType.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(p.PropertyType) == null)
                .Select(p => p.Name);

            schema.Required = schema.Properties.Keys
                .Intersect(nonNulableValueTypedPropNames, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}