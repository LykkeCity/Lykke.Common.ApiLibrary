using System;
using System.Net;
using JetBrains.Annotations;
using Lykke.Common.Api.Contract.Responses;

namespace Lykke.Common.ApiLibrary.Exceptions
{
    [PublicAPI]
    [Serializable]
    public class ValidationApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public ErrorResponse ErrorResponse { get; set; }

        public ValidationApiException(ErrorResponse response) : this(HttpStatusCode.BadRequest, response)
        {
        }
        
        public ValidationApiException(HttpStatusCode code, ErrorResponse response) : base(response.ErrorMessage)
        {
            StatusCode = code;
            ErrorResponse = response;
        }
        
        public ValidationApiException(string message) : this(HttpStatusCode.BadRequest, message)
        {
            ErrorResponse = ErrorResponse.Create(message);
        }
        
        public ValidationApiException(HttpStatusCode code, string message) : base(message)
        {
            StatusCode = code;
            ErrorResponse = ErrorResponse.Create(message);
        }
    }
}
