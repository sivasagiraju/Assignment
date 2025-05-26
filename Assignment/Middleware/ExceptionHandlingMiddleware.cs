using System.Net;
using System.Text.Json;

namespace Assignment.Middleware
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
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)GetStatusCode(ex);

                var response = new
                {
                    message = ex.Message,
                    statusCode = context.Response.StatusCode,
                    error = ex.GetType().Name
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }

        private HttpStatusCode GetStatusCode(Exception ex)
        {
            return ex switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentNullException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
        }
    }
}
