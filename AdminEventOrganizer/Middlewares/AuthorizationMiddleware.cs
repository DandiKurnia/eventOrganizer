namespace AdminEventOrganizer.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var userId = context.Session.GetString("UserId");

            if (path.StartsWith("/login") ||
                path.StartsWith("/register") ||
                path.StartsWith("/landingpage"))
            {
                await _next(context);
                return;
            }

            if (string.IsNullOrEmpty(userId))
            {
                context.Response.Redirect("/login");
                return;
            }

            await _next(context);
        }

    }
}
