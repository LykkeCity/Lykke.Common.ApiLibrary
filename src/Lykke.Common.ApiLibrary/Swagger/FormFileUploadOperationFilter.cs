using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Common.ApiLibrary.Swagger
{
    /// <summary>
    /// Replaces splited or single file upload parameters to 
    /// file upload field in a swagger
    /// </summary>
    public class FormFileUploadOperationFilter : IOperationFilter
    {
        private const string MultipartFormDataMimeType = "multipart/form-data";

        private static readonly string[] FormFilePropertyNames = typeof(IFormFile)
            .GetTypeInfo()
            .DeclaredProperties
            .Select(x => x.Name)
            .ToArray();

        public void Apply(Operation operation, OperationFilterContext context)
        {
            var descriptions = context.ApiDescription.ActionDescriptor.Parameters
                .Where(x => x.ParameterType == typeof(IFormFile))
                .ToArray();

            if (!descriptions.Any())
            {
                return;
            }

            if (!operation.Consumes.Contains(MultipartFormDataMimeType))
            {
                operation.Consumes.Add(MultipartFormDataMimeType);
            }

            var greedyDescriptions = descriptions
                .Where(x => x.BindingInfo?.BindingSource?.IsGreedy == true);
            
            foreach (var description in greedyDescriptions)
            {
                var oldParameter = operation.Parameters.First(o => o.Name == description.Name);
                var parameterIndex = operation.Parameters.IndexOf(oldParameter);

                var newParameter = new NonBodyParameter
                {
                    Name = oldParameter.Name,
                    In = "formData",
                    Description = oldParameter.Description,
                    Required = oldParameter.Required,
                    Type = "file"
                };

                operation.Parameters[parameterIndex] = newParameter;
            }

            var notGreedyDescriptions = descriptions
                .Where(x => x.BindingInfo?.BindingSource?.IsGreedy != true);

            foreach (var description in notGreedyDescriptions)
            {
                var fileParameter = new NonBodyParameter
                {
                    Name = description.Name,
                    In = "formData",
                    Required = false,
                    Type = "file"
                };

                var firstNestedFileParameter = operation
                    .Parameters
                    .OfType<NonBodyParameter>()
                    .FirstOrDefault(x => FormFilePropertyNames.Contains(x.Name));

                if (firstNestedFileParameter == null)
                {
                    break;
                }

                var fileParameterIndex = operation
                    .Parameters
                    .IndexOf(firstNestedFileParameter);

                foreach (var propertyName in FormFilePropertyNames)
                {
                    var parameter = operation.Parameters.First(x => x.Name.Contains(propertyName));
                    operation.Parameters.Remove(parameter);
                }

                operation
                    .Parameters
                    .Insert(fileParameterIndex, fileParameter);
            }
        }
    }
}
