using System;
using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

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

            var invalidParameters = context.ApiDescription.ParameterDescriptions.Where(p => p.Type == null).ToArray();

            if (invalidParameters.Any())
            {
                var invalidParameter = invalidParameters.First();
                var message =
$@"Invalid parameter found. Probably parameter source mismatched. (e.g. You specified [FromQuery] for the path parameter).

Action: {context.ApiDescription?.ActionDescriptor.DisplayName}
Relaive path: {context.ApiDescription?.RelativePath}
OperationId: {operation.OperationId}
Parameter: {invalidParameter?.Name}
Expected parameter source: {invalidParameter?.Source.DisplayName}";
                throw new InvalidOperationException(message);
            }

            foreach (var parameterDescription in context.ApiDescription.ParameterDescriptions.Where(p => p.Type.GetTypeInfo().IsEnum))
            {
                var operationParameter = operation.Parameters.Single(p => p.Name == parameterDescription.Name);

                XmsEnumExtensionApplicator.Apply(operationParameter.Extensions, parameterDescription.Type, _options);
            }
        }
    }
}