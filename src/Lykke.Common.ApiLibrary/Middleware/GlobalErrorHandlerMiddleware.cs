using System;
using System.IO;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Lykke.Common.ApiLibrary.Middleware
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly ILog _log;
        private readonly string _componentName;
        private readonly CreateErrorResponse _createErrorResponse;
        private readonly RequestDelegate _next;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILog log, string componentName, CreateErrorResponse createErrorResponse)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _componentName = componentName ?? throw new ArgumentNullException(nameof(componentName));
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
            // request body might be already read at the moment 
            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            using (var ms = new MemoryStream())
            {
                context.Request.Body.CopyTo(ms);

                ms.Seek(0, SeekOrigin.Begin);

                await _log.LogPartFromStream(ms, _componentName, context.Request.GetUri().AbsoluteUri, ex);
            }
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