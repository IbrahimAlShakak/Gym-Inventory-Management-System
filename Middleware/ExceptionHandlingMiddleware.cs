using Gym_Inventory_Management_System.Exceptions;
using System.Text.Json;

namespace Gym_Inventory_Management_System.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var (statusCode, message) = MapException(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
        }
    }

    private static (int StatusCode, string Message) MapException(Exception exception)
    {
        return exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, exception.Message),
            InvalidOperationBusinessException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidLoginInput => (StatusCodes.Status401Unauthorized, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };
    }

}

