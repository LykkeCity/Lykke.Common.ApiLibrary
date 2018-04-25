using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Common.ApiLibrary.Swagger
{

    /// <summary>
    /// Replaces several operation parameters which mapper by swagger
    /// from IFormFile api parameter to single file upload operation parameter 
    /// </summary>
    public class FileUploadOperationFilter : IOperationFilter
    {
        private const string MultipartFormDataMimeType = "multipart/form-data";

        private static readonly string[] FormFilePropertyNames = typeof(IFormFile)
            .GetTypeInfo()
            .DeclaredProperties
            .Select(x => x.Name)
            .ToArray();

        public void Apply(Operation operation, OperationFilterContext context)
        {
            var fileApiParameters = context
                .ApiDescription
                .ActionDescriptor
                .Parameters
                .Where(x => x.ParameterType == typeof(IFormFile))
                .ToArray();

            if (fileApiParameters.Any())
            {
                if (!operation.Consumes.Contains(MultipartFormDataMimeType))
                {
                    operation.Consumes.Add(MultipartFormDataMimeType);
                }
            }

            foreach (var fileApiParameter in fileApiParameters)
            {
                var fileParameter = new NonBodyParameter
                {
                    Name = fileApiParameter.Name,
                    In = "formData",
                    Required = false,
                    Type = "file"
                };

                var firstNestedFileParameter = operation
                    .Parameters
                    .OfType<NonBodyParameter>()
                    .First(x => FormFilePropertyNames.Contains(x.Name));

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
