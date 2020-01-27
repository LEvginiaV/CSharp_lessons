using System;
using System.Net;

namespace Market.CustomersAndStaff.ServiceApi.Client.Core
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(HttpStatusCode statusCode, string content)
            : base($"Server returned {statusCode:D} ({statusCode:G}). Content: {(string.IsNullOrEmpty(content) ? "EMPTY" : content)}")
        {
            StatusCode = statusCode;
            Content = content;
        }

        public HttpStatusCode StatusCode { get; }
        public string Content { get; }
    }
}