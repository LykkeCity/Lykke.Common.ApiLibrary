using System;
using System.Net;
using JetBrains.Annotations;

namespace Lykke.Common.ApiLibrary.Exceptions
{
    [PublicAPI]
    [Serializable]
    public class ValidationApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public ValidationApiException(string message) : this(HttpStatusCode.BadRequest, message)
        {
        }
        
        public ValidationApiException(HttpStatusCode code, string message) : base(message)
        {
            StatusCode = code;
        }
    }
}
