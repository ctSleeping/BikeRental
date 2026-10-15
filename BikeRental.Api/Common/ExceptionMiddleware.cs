using System.Text.Json;

namespace BikeRental.Api.Common;

// Converts exceptions into ProblemDetails responses with a "code" field.
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            await WriteProblem(context, ex.StatusCode, ex.Message, ex.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblem(context, 500, "An unexpected error occurred.", "InternalServerError");
        }
    }

    private static Task WriteProblem(HttpContext context, int status, string detail, string code)
    {
        var title = status switch
        {
            404 => "Not Found",
            409 => "Conflict",
            _ => "Internal Server Error"
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var body = new { title, status, detail, code };
        return context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }
}