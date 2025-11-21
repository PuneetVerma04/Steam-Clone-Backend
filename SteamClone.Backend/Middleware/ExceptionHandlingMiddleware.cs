using System.Net;
using System.Text.Json;

namespace SteamClone.Backend.Middleware
{
  public class ExceptionHandlingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
        await HandleExceptionAsync(context, ex);
      }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      if (context.Response.HasStarted)
      {
        throw exception;
      }

      context.Response.Clear();
      context.Response.ContentType = "application/json";

      // Set status code based on exception type
      context.Response.StatusCode = exception switch
      {
        ArgumentException => (int)HttpStatusCode.BadRequest,
        UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
        KeyNotFoundException => (int)HttpStatusCode.NotFound,
        InvalidOperationException => (int)HttpStatusCode.Conflict,
        _ => (int)HttpStatusCode.InternalServerError
      };
      var response = new
      {
        error = new
        {
          message = exception.Message,
          type = exception.GetType().Name
        }
      };

      var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      });

      await context.Response.WriteAsync(jsonResponse);
    }
  }
}
