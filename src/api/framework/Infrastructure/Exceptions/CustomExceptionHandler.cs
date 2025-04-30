using imediatus.Framework.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace imediatus.Framework.Infrastructure.Exceptions;
public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);
        var problemDetails = new ProblemDetails();
        problemDetails.Instance = httpContext.Request.Path;

        if (exception is FluentValidation.ValidationException fluentException)
        {
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            List<string> validationErrors = fluentException.Errors.Select(error => error.ErrorMessage).ToList();
            problemDetails.Detail = validationErrors.Count.Equals(0) ? "one or more validation errors occurred" : string.Join(" ", validationErrors);
            problemDetails.Extensions.Add("errors", validationErrors);
        }

        else if (exception is ImediatusException e)
        {
            httpContext.Response.StatusCode = (int)e.StatusCode;
            problemDetails.Detail = e.Message;
            if (e.ErrorMessages != null && e.ErrorMessages.Any())
            {
                problemDetails.Extensions.Add("errors", e.ErrorMessages);
            }
        }

        else
        {
            problemDetails.Detail = exception.Message;
        }

        LogContext.PushProperty("StackTrace", exception.StackTrace);
        logger.LogError("{ProblemDetail}", problemDetails.Detail);
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }
}
