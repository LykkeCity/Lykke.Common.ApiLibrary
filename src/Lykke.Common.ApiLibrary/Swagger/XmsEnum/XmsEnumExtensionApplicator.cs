using System;
using System.Collections.Generic;

namespace Lykke.Common.ApiLibrary.Swagger.XmsEnum
{
    internal static class XmsEnumExtensionApplicator
    {
        public static void Apply(IDictionary<string, object> extensions, Type enumType, XmsEnumExtensionsOptions options)
        {
            extensions.Add("x-ms-enum", new
            {
                name = enumType.Name,
                modelAsString = options != XmsEnumExtensionsOptions.UseEnums
            });
        }
    }
}