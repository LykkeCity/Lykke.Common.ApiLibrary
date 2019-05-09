using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Common.ApiLibrary.Filters
{
    /// <summary>
    /// Middleware that handles responses with no content.
    /// </summary>
    [PublicAPI]
    public class NoContentFilter : IResultFilter
    {
        private readonly ILog _log;

        public NoContentFilter(ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            var response = context.HttpContext.Response;
            if (response.StatusCode == StatusCodes.Status200OK && (response.ContentLength ?? 0) == 0)
            {
                var okResult = context.Result as ObjectResult;
                if (okResult?.Value == null)
                {
                    if (response.HasStarted)
                        _log.Warning($"Couldn't set NoContent into response to {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
                    else
                        response.StatusCode = StatusCodes.Status204NoContent;
                }
            }
        }
    }
}
