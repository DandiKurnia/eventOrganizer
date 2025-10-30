using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdminEventOrganizer.Filters
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var currentPath = context.HttpContext.Request.Path;

            // Skip authorization untuk login page dan public routes
            if (currentPath.StartsWithSegments("/login") ||
                currentPath.StartsWithSegments("/register") ||
                currentPath.StartsWithSegments("/Home"))
            {
                return;
            }

            var role = context.HttpContext.Session.GetString("Role");
            var userId = context.HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId) || role != "Admin")
            {
                context.Result = new RedirectToActionResult("Login", "User", new
                {
                    returnUrl = context.HttpContext.Request.Path
                });
            }
        }
    }
}
