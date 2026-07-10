using System.Text.Json;

namespace WorkTimeTracker.API.Middleware;

/// <summary>
/// Middleware для глобальной обработки исключений
/// </summary>
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
            _logger.LogError(ex, "Произошла ошибка при обработке запроса");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            StatusCode = 500,
            Message = "Внутренняя ошибка сервера",
            Detail = exception.Message,
            Timestamp = DateTime.UtcNow
        };
        
        // Обработка специфических исключений
        if (exception is InvalidOperationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = new
            {
                StatusCode = 400,
                Message = "Ошибка валидации",
                Detail = exception.Message,
                Timestamp = DateTime.UtcNow
            };
        }
        else if (exception is KeyNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            response = new
            {
                StatusCode = 404,
                Message = "Ресурс не найден",
                Detail = exception.Message,
                Timestamp = DateTime.UtcNow
            };
        }
        else if (exception is ArgumentException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = new
            {
                StatusCode = 400,
                Message = "Некорректные аргументы",
                Detail = exception.Message,
                Timestamp = DateTime.UtcNow
            };
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
        
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        await context.Response.WriteAsync(jsonResponse);
    }
}