namespace StaffEventOrganizer.Middlewares
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
            var path = context.Request.Path.Value?.ToLower();
            var userId = context.Session.GetString("UserId");

            // allow login, register, static
            if (path.Contains("/login") ||
                path.Contains("/register") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/lib"))
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
