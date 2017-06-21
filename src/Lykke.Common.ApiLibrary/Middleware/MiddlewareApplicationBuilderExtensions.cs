using Microsoft.AspNetCore.Builder;

namespace Lykke.Common.ApiLibrary.Middleware
{
    public static class MiddlewareApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure application to use standart Lykke middleware
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="componentName">Component name for logs</param>
        /// <param name="createGlobalErrorResponse">Create global error response delegate</param>
        public static void UseLykkeMiddleware(this IApplicationBuilder app, string componentName, CreateErrorResponse createGlobalErrorResponse)
        {
            app.UseMiddleware<GlobalErrorHandlerMiddleware>(componentName, createGlobalErrorResponse);
        }
    }
}