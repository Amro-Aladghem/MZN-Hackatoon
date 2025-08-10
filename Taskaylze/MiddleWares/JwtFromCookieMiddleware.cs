namespace Taskalayze.MiddleWares
{
    public class JwtFromCookieMiddleware
    {
        private readonly RequestDelegate _next; 

        public JwtFromCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("AuthToken", out var token))
            {
                context.Request.Headers.Authorization = $"Bearer {token}";
            }

            await _next(context);
        }
    }
}
