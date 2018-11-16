using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Common.ApiLibrary.Swagger.XmsEnum
{
    /// <summary>
    /// Warning. It affects not only responses but also requests
    /// </summary>
    [UsedImplicitly]
    internal class ResponseValueTypesRequiredSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            if (schema.Type != "object" || schema.Properties == null)
            {
                return;
            }

            var nonNullableValueTypedPropNames = context.SystemType.GetProperties()
                .Where(p =>
                    // is it value type?
                    p.PropertyType.GetTypeInfo().IsValueType &&
                    // is it not nullable type?
                    Nullable.GetUnderlyingType(p.PropertyType) == null &&
                    // is it read/write property
                    p.CanRead && p.CanWrite)
                .Select(p => p.Name);

            schema.Required = schema.Required == null
                ? nonNullableValueTypedPropNames.ToList()
                : schema.Required.Union(nonNullableValueTypedPropNames, StringComparer.OrdinalIgnoreCase).ToList();

            if (!schema.Required.Any())
            {
                schema.Required = null;
            }
        }
    }
}