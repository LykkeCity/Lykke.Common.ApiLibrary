using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;

namespace Lykke.Common.ApiLibrary.Middleware
{
    public class ClientErrorHandlerMiddleware
    {
        private readonly ILog _log;
        private readonly string _componentName;
        private readonly RequestDelegate _next;

        public ClientErrorHandlerMiddleware(RequestDelegate next, ILog log, string componentName)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _componentName = componentName ?? throw new ArgumentNullException(nameof(componentName));
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);

            if (context.Response.StatusCode >= StatusCodes.Status400BadRequest && 
                context.Response.StatusCode <= StatusCodes.Status451UnavailableForLegalReasons)
            {
                await LogWarning(context);
            }
        }

        private async Task LogWarning(HttpContext context)
        {
            var body = string.Empty;

            if (context.Request.Body.CanSeek &&
                context.Request.Body.Length > 0)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);

                // max Azure table column size is 64k
                var size = 64 * 1024; 
                var data = await context.Request.Body.ReadAsMuchAsPossible(size);

                body = Encoding.UTF8.GetString(data);
            }

            await _log.WriteWarningAsync(_componentName, context.Request.GetUri().AbsoluteUri, body,
                $"Client error {context.Response.StatusCode}");
        }
    }
}