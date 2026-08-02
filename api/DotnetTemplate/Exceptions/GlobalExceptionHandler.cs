using DotnetTemplate.Responses;
using Microsoft.AspNetCore.Diagnostics;

namespace DotnetTemplate.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public GlobalExceptionHandler()
    {
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            ApiException apiException => apiException.StatusCode,

            _ => StatusCodes.Status500InternalServerError
        };

        var message = statusCode >= 500 && exception is not ApiException
            ? "Ocorreu um erro interno na aplicação."
            : exception.Message;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ErrorResponse
            {
                Message = message,
                Status = statusCode
            },
            cancellationToken);

        return true;
    }
}
