using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Lykke.Common.ApiLibrary.Swagger.XmsEnum
{
    internal static class XmsEnumExtensionApplicator
    {
        public static void Apply(IDictionary<string, IOpenApiExtension> extensions, Type enumType, XmsEnumExtensionsOptions options)
        {
            extensions.Add(
                "x-ms-enum",
                new OpenApiObject
                {
                    ["name"] = enumType.GetTypeInfo().IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        ? new OpenApiString(enumType.GetGenericArguments()[0].Name)
                        : new OpenApiString(enumType.Name),
                    ["modelAsString"] = new OpenApiBoolean(options != XmsEnumExtensionsOptions.UseEnums)
                }
            );
        }

        public static void Apply(IDictionary<string, IOpenApiExtension> extensions, string typeName, XmsEnumExtensionsOptions options)
        {
            extensions.Add(
                "x-ms-enum",
                new OpenApiObject
                {
                    ["name"] = new OpenApiString(typeName),
                    ["modelAsString"] = new OpenApiBoolean(options != XmsEnumExtensionsOptions.UseEnums)
                }
            );
        }
    }
}