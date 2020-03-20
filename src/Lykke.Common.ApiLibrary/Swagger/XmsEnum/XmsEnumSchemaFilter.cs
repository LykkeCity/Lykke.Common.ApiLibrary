using System.Linq;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
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

        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (model.Properties == null)
                return;

            foreach (var property in model.Properties.Where(x => x.Value.Enum != null))
            {
                XmsEnumExtensionApplicator.Apply(property.Value.Extensions, property.Value.Type, _options);
            }
        }
    }
}
