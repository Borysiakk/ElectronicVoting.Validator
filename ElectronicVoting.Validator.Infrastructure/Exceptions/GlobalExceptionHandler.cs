using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
using IProblemDetailsService = Microsoft.AspNetCore.Http.IProblemDetailsService;

namespace ElectronicVoting.Validator.Infrastructure.Exceptions;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";
        var excDetails = exception switch
        {
            ValidationAppException => (Detail: exception.Message, StatusCodes: Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity),
            _ => (Detail: exception.Message, StatusCodes: StatusCodes.Status500InternalServerError)
        };
        httpContext.Response.StatusCode = excDetails.StatusCodes;

        if (exception is ValidationAppException validationException)
        {
            await httpContext.Response.WriteAsJsonAsync(new { validationException.Errors }, cancellationToken: cancellationToken);
            return true;
        }
        
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Title = "An error occurred",
                Detail = excDetails.Detail,
                Type = exception.GetType().Name,
                Status = excDetails.StatusCodes
            },
            Exception = exception,
        });
    }
}