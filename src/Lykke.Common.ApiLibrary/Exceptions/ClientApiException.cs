using System;
using System.Net;
using JetBrains.Annotations;
using Lykke.Common.Api.Contract.Responses;

namespace Lykke.Common.ApiLibrary.Exceptions
{
    /// <summary>
    /// client api exception
    /// </summary>
    [PublicAPI]
    [Serializable]
    public class ClientApiException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public ErrorResponse ErrorResponse { get; set; }
        
        public ClientApiException(HttpStatusCode statusCode, ErrorResponse errorResponse):base(errorResponse.ErrorMessage)
        {
            HttpStatusCode = statusCode;
            ErrorResponse = errorResponse;
        }
    }
}
