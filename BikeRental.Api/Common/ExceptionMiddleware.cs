using System.Text.Json;

namespace BikeRental.Api.Common;

// Runs around every request. If anything below it throws,
// it turns the exception into a ProblemDetails JSON response.
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
            await _next(context);   // run the rest of the pipeline (controllers etc.)
        }
        catch (ApiException ex)
        {
            // Expected error from our own rules -> its own status + code
            await WriteProblem(context, ex.StatusCode, ex.Message, ex.Code);
        }
        catch (Exception ex)
        {
            // Unexpected bug -> log it, return a generic 500
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