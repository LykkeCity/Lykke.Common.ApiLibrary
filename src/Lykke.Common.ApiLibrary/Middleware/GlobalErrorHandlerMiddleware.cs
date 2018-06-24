using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Lykke.Common.ApiLibrary.Middleware
{
    /// <summary>
    /// Middleware that handles all unhandled exceptions and use delegate to generate error response
    /// </summary>
    [PublicAPI]
    public class GlobalErrorHandlerMiddleware
    {
        private readonly ILog _log;
        private readonly CreateErrorResponse _createErrorResponse;
        private readonly RequestDelegate _next;

        /// <summary>
        /// Middleware that handles all unhandled exceptions and use delegate to generate error response
        /// </summary>
        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogFactory logFactory, CreateErrorResponse createErrorResponse)
        {
            if (logFactory == null)
            {
                throw new ArgumentNullException(nameof(logFactory));
            }

            _log = logFactory.CreateLog(this);
            _createErrorResponse = createErrorResponse ?? throw new ArgumentNullException(nameof(createErrorResponse));
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await LogError(context, ex);
                await CreateErrorResponse(context, ex);
            }
        }

        private async Task LogError(HttpContext context, Exception ex)
        {
            var url = context.Request?.GetUri()?.AbsoluteUri;
            var urlWithoutQuery = RequestUtils.GetUrlWithoutQuery(url) ?? "?";
            var body = await RequestUtils.GetRequestPartialBodyAsync(context);

            _log.Error(urlWithoutQuery, ex, context: new
            {
                url = url,
                body = body
            });
        }

        private async Task CreateErrorResponse(HttpContext ctx, Exception ex)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = 500;

            var response = _createErrorResponse(ex);
            var responseJson = JsonConvert.SerializeObject(response);

            await ctx.Response.WriteAsync(responseJson);
        }
    }
}