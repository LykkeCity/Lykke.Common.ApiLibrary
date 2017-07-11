using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lykke.Common.ApiLibrary.Swagger.XmsEnum
{
    internal static class XmsEnumExtensionApplicator
    {
        public static void Apply(IDictionary<string, object> extensions, Type enumType, XmsEnumExtensionsOptions options)
        {
            extensions.Add("x-ms-enum", new
            {
                name = enumType.GetTypeInfo().IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                    enumType.GetGenericArguments()[0].Name :
                    enumType.Name,
                modelAsString = options != XmsEnumExtensionsOptions.UseEnums
            });
        }
    }
}