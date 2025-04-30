using System.Collections.ObjectModel;
using System.Net;

namespace imediatus.Framework.Core.Exceptions;

public class CustomException(string message, ReadOnlyCollection<string>? errors = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : Exception(message)
{
    public ReadOnlyCollection<string>? ErrorMessages { get; } = errors;
    public HttpStatusCode StatusCode { get; } = statusCode;
}
