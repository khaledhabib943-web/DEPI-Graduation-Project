using Microsoft.AspNetCore.Http;

namespace FinalProject.Web.Middleware
{
    /// <summary>
    /// Reads the 'salahly_lang' cookie and automatically redirects the user
    /// to the Arabic (Ar-suffixed) action when their preferred language is Arabic,
    /// or to the English action when their preference is English.
    /// This ensures language preference persists across page navigations and refreshes.
    /// </summary>
    public class LanguageRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        // Controllers/actions that should never be auto-redirected
        private static readonly HashSet<string> _excludedControllers = new(StringComparer.OrdinalIgnoreCase)
        {
            "Account", // Login, Register, ForgotPassword — already have Ar variants handled differently
        };

        // Paths that should never be redirected (API calls, static files, Identity framework)
        private static readonly string[] _excludedPrefixes = new[]
        {
            "/Identity",
            "/api",
            "/_blazor",
            "/favicon",
            "/css",
            "/js",
            "/lib",
            "/images",
            "/uploads",
            "/Account/ExternalLogin",
            "/Account/ExternalRegister",
            "/Account/ExternalCallback",
            "/Account/Logout",
        };

        public LanguageRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "/";

            // Skip static files, API calls, and excluded paths
            if (ShouldSkip(path))
            {
                await _next(context);
                return;
            }

            // Read language cookie
            var lang = context.Request.Cookies["salahly_lang"];

            if (string.IsNullOrEmpty(lang))
            {
                // No preference set — let the request pass through normally
                await _next(context);
                return;
            }

            var segments = path.TrimStart('/').Split('/', 3);
            // segments[0] = Controller, segments[1] = Action (optional), segments[2] = Id (optional)

            var controller = segments.Length > 0 ? segments[0] : string.Empty;
            var action = segments.Length > 1 ? segments[1] : "Index";

            // Skip excluded controllers
            if (_excludedControllers.Contains(controller))
            {
                await _next(context);
                return;
            }

            var isCurrentlyAr = action.EndsWith("Ar", StringComparison.OrdinalIgnoreCase);

            if (lang == "ar" && !isCurrentlyAr)
            {
                // Redirect to the Ar version
                var newAction = action + "Ar";
                var newPath = BuildPath(segments, controller, newAction);
                var redirectUrl = newPath + context.Request.QueryString.Value;
                context.Response.Redirect(redirectUrl, permanent: false);
                return;
            }

            if (lang == "en" && isCurrentlyAr)
            {
                // Redirect to the English version
                var newAction = action.Length > 2 ? action[..^2] : "Index"; // Remove "Ar"
                if (string.IsNullOrEmpty(newAction)) newAction = "Index";
                var newPath = BuildPath(segments, controller, newAction);
                var redirectUrl = newPath + context.Request.QueryString.Value;
                context.Response.Redirect(redirectUrl, permanent: false);
                return;
            }

            await _next(context);
        }

        private static string BuildPath(string[] segments, string controller, string action)
        {
            if (segments.Length >= 3)
                return $"/{controller}/{action}/{segments[2]}";
            return $"/{controller}/{action}";
        }

        private static bool ShouldSkip(string path)
        {
            // Skip empty or root
            if (path == "/" || string.IsNullOrEmpty(path))
                return true;

            // Skip excluded prefixes
            foreach (var prefix in _excludedPrefixes)
            {
                if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // Skip files with extensions (static assets)
            var lastSegment = path.Split('/').Last();
            if (lastSegment.Contains('.'))
                return true;

            return false;
        }
    }
}
