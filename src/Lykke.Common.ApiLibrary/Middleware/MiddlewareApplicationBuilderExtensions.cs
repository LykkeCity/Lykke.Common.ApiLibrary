using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Internal;

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
        public static void UseLykkeMiddleware(this IApplicationBuilder app, string componentName, CreateErrorResponse createGlobalErrorResponse,
            bool logClientErrors = false)
        {
            app.Use(async (context, next) =>
            {
                // enable ability to seek on request stream within any host,
                // but not only Kestrel, for any subsequent middleware
                context.Request.EnableRewind();
                await next();
            });

            app.UseMiddleware<GlobalErrorHandlerMiddleware>(componentName, createGlobalErrorResponse);

            if (logClientErrors)
            {
                app.UseMiddleware<ClientErrorHandlerMiddleware>(componentName);
            }
        }
    }
}