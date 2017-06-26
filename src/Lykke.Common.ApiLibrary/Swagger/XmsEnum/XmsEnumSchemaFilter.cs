using System.Linq;
using System.Reflection;
using Lykke.Common.ApiLibrary.Swagger.XmsEnum;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

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

            foreach (var property in model.Properties.Where(x => x.Value.Enum != null))
            {
                var propertyType = context.SystemType
                    .GetProperty(property.Key, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)
                    .PropertyType;

                XmsEnumExtensionApplicator.Apply(property.Value.Extensions, propertyType, _options);
            }
        }
    }
}