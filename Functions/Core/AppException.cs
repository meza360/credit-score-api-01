using System.Net;

namespace Functions.Core
{
    public class AppException
    {
        public AppException(HttpStatusCode statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
    }
}