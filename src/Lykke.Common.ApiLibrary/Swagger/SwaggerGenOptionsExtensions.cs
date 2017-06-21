using Swashbuckle.SwaggerGen.Application;

namespace Lykke.Common.ApiLibrary.Swagger
{
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// Enables "x-ms-enum" swagger extension, wich allows Autorest tool generates enum or set of string constants for each server-side enum.
        /// </summary>
        /// <param name="swaggerOptions"></param>
        /// <param name="options">"x-ms-enum" extensions options. Default value is <see cref="XmsEnumExtensionsOptions.UseEnums"/></param>
        public static void EnableXmsEnumExtension(this SwaggerGenOptions swaggerOptions, XmsEnumExtensionsOptions options = XmsEnumExtensionsOptions.UseEnums)
        {
            swaggerOptions.SchemaFilter<XmsEnumSchemaFilter>(options);
        }
    }
}