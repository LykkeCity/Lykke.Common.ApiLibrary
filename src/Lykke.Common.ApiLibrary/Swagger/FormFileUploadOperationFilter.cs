using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Common.ApiLibrary.Swagger
{
    /// <summary>
    /// Replaces string input operation parameter which mapper by swagger
    /// from IFormFile api parameter to file upload operation parameter.
    /// 
    /// IFormFile argument of controller's method should not be marked
    /// by binding attribute such as [FromForm], [FromBody] or etc.
    /// </summary>
    public class FormFileUploadOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var descriptions = context.ApiDescription.ParameterDescriptions
                .Where(x => x.ParameterDescriptor.ParameterType == typeof(IFormFile))
                .ToArray();

            if (descriptions.Any(x => x.Source.Id != "FormFile"))
            {
                throw new InvalidOperationException("IFormFile argument of controller's method should not be marked " +
                                                    "by binding attribute such as [FromForm], [FromBody] or etc. " +
                                                    "to be compatible with swagger");
            }

            if (descriptions.Any())
            {
                operation.Consumes.Add("application/form-data");
            }

            foreach (var description in descriptions)
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
        }
    }
}
