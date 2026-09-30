using FerryBooking.Api.Domain;
using Microsoft.AspNetCore.Diagnostics;

namespace FerryBooking.Api.Common;

/// <summary>
/// Turns a <see cref="DomainException"/> into an RFC 9457 problem details response
/// so business-rule violations reach the client as 422 instead of 500.
/// </summary>
internal sealed class DomainExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Title = "Business rule violated",
                Detail = domainException.Message,
                Status = StatusCodes.Status422UnprocessableEntity,
            },
        });
    }
}
