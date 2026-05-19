using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace WebApi.Handlers;

using AppCore.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;


public class ProblemDetailsExceptionHandler(
    ProblemDetailsFactory factory, ILogger<ProblemDetailsExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is GateNotFoundException)
        {
            logger.Log(LogLevel.Information, $"Exception '{exception.Message}' handled!");
            
            var problem = factory.CreateProblemDetails(
                context,
                StatusCodes.Status404NotFound,
                "Zasób nie istnieje",
                "Gate error",
                detail: exception.Message
            );
            
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
        return false;
    }
}