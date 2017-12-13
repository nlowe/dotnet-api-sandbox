using System;
using System.Net;

namespace MyApp.Exceptions
{
    public class ApiException : Exception
    {
        public readonly HttpStatusCode ResponseCode;

        public ApiException(HttpStatusCode status, string message) : base(message) => ResponseCode = status;
    }
}