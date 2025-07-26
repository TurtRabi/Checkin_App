using System.Net;
using System.Text.Json;
using Common;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomException ex)
        {
            await HandleExceptionAsync(context, ex.StatusCode, ex.ErrorCode, ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, "HB40101", "Token missing or invalid");
        }
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.NotFound, "HB40401", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error");

            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "HB50001", "Internal server error");
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string errorCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new
        {
            errorCode,
            message
        });

        return context.Response.WriteAsync(result);
    }
}