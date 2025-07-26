using System;
using System.Net;

namespace Common
{
    public class CustomException : Exception
    {
        public string ErrorCode { get; }
        public HttpStatusCode StatusCode { get; }

        public CustomException(string errorCode, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}