using System.Net;
using System.Text.Json;

namespace JobScrawler.ExceptionHandling;

public class ExceptionMiddleware
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    //! It has to be named InvokeAsync
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            //! Remember, when we are out of our flow, we have to specify what response we expect
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _env.IsDevelopment()
                // If we are in development, we will need more specific and in detailed error message
                ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace)
                // If we are not in development, we will just show a simple internal server error message
                : new AppException(context.Response.StatusCode, "Internal Server Error");

            // Outside the API or flow, we have to specify it ourselves
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}