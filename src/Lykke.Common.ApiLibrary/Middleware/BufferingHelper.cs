using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Lykke.Common.ApiLibrary.Middleware
{
    internal static class BufferingHelper
    {
        private const int DefaultBufferThreshold = 1024 * 30;

        private static string _tempDirectory;

        public static HttpRequest EnableRewind(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var body = request.Body;
            if (!body.CanSeek)
            {
                var fileStream = new FileBufferingReadStream(body, DefaultBufferThreshold, null, GetTempDirectory);
                request.Body = fileStream;
                request.HttpContext.Response.RegisterForDispose(fileStream);
            }

            return request;
        }

        private static string GetTempDirectory()
        {
            if (_tempDirectory != null)
                return _tempDirectory;

            // Look for folders in the following order.
            var temp = Environment.GetEnvironmentVariable("ASPNETCORE_TEMP") // ASPNETCORE_TEMP - User set temporary location.
                ?? Path.GetTempPath();                                       // Fall back.

            if (!Directory.Exists(temp))
            {
                // TODO: ???
                throw new DirectoryNotFoundException(temp);
            }

            _tempDirectory = temp;

            return _tempDirectory;
        }
    }
}
