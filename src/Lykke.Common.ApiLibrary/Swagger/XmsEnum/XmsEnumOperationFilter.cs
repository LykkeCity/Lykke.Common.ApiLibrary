using System.Linq;
using System.Reflection;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

namespace Lykke.Common.ApiLibrary.Swagger.XmsEnum
{
    public class XmsEnumOperationFilter : IOperationFilter
    {
        private readonly XmsEnumExtensionsOptions _options;

        public XmsEnumOperationFilter(XmsEnumExtensionsOptions options)
        {
            _options = options;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            foreach (var parameterDescription in context.ApiDescription.ParameterDescriptions.Where(p => IntrospectionExtensions.GetTypeInfo(p.Type).IsEnum))
            {
                var operationParameter = operation.Parameters.Single(p => p.Name == parameterDescription.Name);

                XmsEnumExtensionApplicator.Apply(operationParameter.Extensions, parameterDescription.Type, _options);
            }
        }
    }
}