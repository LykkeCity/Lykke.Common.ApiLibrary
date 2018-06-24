using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;

namespace Lykke.Common.ApiLibrary.Middleware
{
    /// <summary>
    /// Logs url, request body and response status for responses with status code = 4xx
    /// </summary>
    [PublicAPI]
    public class ClientErrorHandlerMiddleware
    {
        private readonly ILog _log;
        private readonly RequestDelegate _next;

        /// <summary>
        /// Logs url, request body and response status for responses with status code = 4xx
        /// </summary>
        public ClientErrorHandlerMiddleware(RequestDelegate next, ILogFactory logFactory)
        {
            if (logFactory == null)
            {
                throw new ArgumentNullException(nameof(logFactory));
            }

            _log = logFactory.CreateLog(this);
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);

            if (context.Response.StatusCode >= StatusCodes.Status400BadRequest && 
                context.Response.StatusCode <= StatusCodes.Status451UnavailableForLegalReasons)
            {
                await LogError(context);
            }
        }

        private async Task LogError(HttpContext context)
        {
            var url = context.Request?.GetUri()?.AbsoluteUri;
            var urlWithoutQuery = RequestUtils.GetUrlWithoutQuery(url) ?? "?";
            var body = await RequestUtils.GetRequestPartialBodyAsync(context);

            _log.Warning(urlWithoutQuery, message: null, context: new
            {
                url = url,
                statusCode = context.Response.StatusCode,
                body = body
            });
        }
    }
}