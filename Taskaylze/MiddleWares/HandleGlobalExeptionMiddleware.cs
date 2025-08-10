using System.Text.Json;
namespace Taskalayze.MiddleWares
{
    public class HandleGlobalExeptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HandleGlobalExeptionMiddleware> _logger;

        public HandleGlobalExeptionMiddleware(RequestDelegate next, ILogger<HandleGlobalExeptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing the request.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Server Error,please try again"
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
