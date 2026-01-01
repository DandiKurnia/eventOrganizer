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
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var userId = context.Session.GetString("UserId");
            var role = context.Session.GetString("Role");

            // Izinkan hanya halaman public: root (/), login, register, logout, dan landingpage
            if (path == "/" || 
                path.StartsWith("/login") || 
                path.StartsWith("/register") ||
                path.StartsWith("/logout") ||
                path.StartsWith("/landingpage") ||
                path.StartsWith("/file"))  // Allow file access for all authenticated users
            {
                await _next(context);
                return;
            }

            // Jika belum login, redirect ke /login
            if (string.IsNullOrEmpty(userId))
            {
                context.Response.Redirect("/login");
                return;
            }

            // ===== ROLE-BASED AUTHORIZATION =====
            // Check if user is trying to access a route that doesn't match their role
            if (!string.IsNullOrEmpty(role))
            {
                // Customer hanya bisa akses /customer routes
                if (role == "Customer" && !path.StartsWith("/customer"))
                {
                    context.Response.Redirect("/customer");
                    return;
                }

                // Vendor hanya bisa akses /vendor routes
                if (role == "Vendor" && !path.StartsWith("/vendor"))
                {
                    context.Response.Redirect("/vendor");
                    return;
                }

                // Staff hanya bisa akses /staff routes
                if (role == "Staff" && !path.StartsWith("/staff"))
                {
                    context.Response.Redirect("/staff");
                    return;
                }
            }

            // Jika sudah login dan role sesuai, lanjutkan request
            await _next(context);
        }
    }
}
