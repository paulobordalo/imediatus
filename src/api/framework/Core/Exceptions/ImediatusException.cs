using System.Net;

namespace imediatus.Framework.Core.Exceptions;
public class ImediatusException : Exception
{
    public IEnumerable<string> ErrorMessages { get; }

    public HttpStatusCode StatusCode { get; }

    public ImediatusException(string message, IEnumerable<string> errors, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }

    public ImediatusException(string message) : base(message)
    {
        ErrorMessages = new List<string>();
    }
}
