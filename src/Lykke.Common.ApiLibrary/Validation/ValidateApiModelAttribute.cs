using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Lykke.Common.ApiLibrary.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lykke.Common.ApiLibrary.Validation
{
    /// <summary>
    /// Attribute to validate Public API data models
    /// </summary>
    public class ValidateApiModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                var parameters = descriptor.MethodInfo.GetParameters();

                foreach (var parameter in parameters)
                {
                    var argument = context.ActionArguments.ContainsKey(parameter.Name)
                        ? context.ActionArguments[parameter.Name]
                        : null;

                    EvaluateValidationAttributes(parameter, argument, context.ModelState);
                }
            }

            base.OnActionExecuting(context);

            if (!context.ModelState.IsValid)
            {
                var apiError = LykkeApiCommonErrorCodes.InvalidInput;

                context.Result = new BadRequestObjectResult(new LykkeApiErrorResponse
                {
                    Error = apiError.Name,
                    Message = apiError.DefaultMessage
                });
            }
        }

        private void EvaluateValidationAttributes(ParameterInfo parameter, object argument, ModelStateDictionary modelState)
        {
            var validationAttributes = parameter.CustomAttributes;

            foreach (var attributeData in validationAttributes)
            {
                var attributeInstance = parameter.GetCustomAttribute(attributeData.AttributeType);

                if (attributeInstance is ValidationAttribute validationAttribute)
                {
                    var isValid = validationAttribute.IsValid(argument);

                    if (!isValid)
                    {
                        modelState.AddModelError(parameter.Name,
                            validationAttribute.FormatErrorMessage(parameter.Name));
                    }
                }
            }
        }
    }
}