using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.HttpOverrides;

namespace Lykke.Common.ApiLibrary.Middleware
{
    [PublicAPI]
    public static class MiddlewareApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure application to use standart Lykke middleware
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="createGlobalErrorResponse">Create global error response delegate</param>
        /// <param name="logClientErrors">Enables logging of the requests with 4xx response codes</param>
        public static void UseLykkeMiddleware(this IApplicationBuilder app, CreateErrorResponse createGlobalErrorResponse,
            bool logClientErrors = false)
        {
            app.Use(async (context, next) =>
            {
                // enable ability to seek on request stream within any host,
                // but not only Kestrel, for any subsequent middleware
                context.Request.EnableRewind();
                await next();
            });

            app.UseMiddleware<GlobalErrorHandlerMiddleware>(createGlobalErrorResponse);

            if (logClientErrors)
            {
                app.UseMiddleware<ClientErrorHandlerMiddleware>();
            }
        }
        
        /// <summary>
        /// Configure application to use forwarded headers
        /// </summary>
        /// <param name="app">Application builder</param>
        public static void UseLykkeForwardedHeaders(this IApplicationBuilder app)
        {
            var forwardingOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            };
                
            forwardingOptions.KnownNetworks.Clear(); //its loopback by default
            forwardingOptions.KnownProxies.Clear();
            
            app.UseForwardedHeaders(forwardingOptions);
        }
    }
}
