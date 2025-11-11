using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EventOrganizer.Middlewares
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
            var path = context.Request.Path.ToString().ToLower();
            var userId = context.Session.GetString("UserId");
            var role = context.Session.GetString("Role");

            // Izinkan halaman login dan register
            if (path.Contains("/login") || path.Contains("/register"))
            {
                await _next(context);
                return;
            }
            if (path.StartsWith("/"))
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
