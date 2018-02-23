using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lykke.Common.ApiLibrary.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static string GetErrorMessage(this ModelStateDictionary modelState)
        {
            foreach (var state in modelState)
            {
                var message = state.Value.Errors
                    .Select(e =>
                        string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? e.Exception?.Message
                            : e.ErrorMessage
                    ).FirstOrDefault(m => m != null);

                if (string.IsNullOrEmpty(message))
                    continue;

                return message;
            }

            return string.Empty;
        }
    }
}
