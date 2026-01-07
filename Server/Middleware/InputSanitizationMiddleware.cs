using System.Text.RegularExpressions;

namespace Server.Middleware
{
    public class InputSanitizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<InputSanitizationMiddleware> _logger;

        public InputSanitizationMiddleware(RequestDelegate next, ILogger<InputSanitizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check for potentially malicious patterns in query strings
            if (context.Request.QueryString.HasValue)
            {
                var queryString = context.Request.QueryString.Value;
                if (ContainsMaliciousPatterns(queryString))
                {
                    _logger.LogWarning("Potentially malicious query string detected: {QueryString}", queryString);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid request");
                    return;
                }
            }

            // Enable request body buffering for reading
            context.Request.EnableBuffering();

            await _next(context);
        }

        private bool ContainsMaliciousPatterns(string input)
        {
            // Check for common SQL injection patterns
            var sqlPatterns = new[]
            {
            @"(\%27)|(\')|(\-\-)|(\%23)|(#)",
            @"((\%3D)|(=))[^\n]*((\%27)|(\')|(\-\-)|(\%3B)|(;))",
            @"\w*((\%27)|(\'))((\%6F)|o|(\%4F))((\%72)|r|(\%52))",
            @"((\%27)|(\'))union"
        };

            // Check for XSS patterns
            var xssPatterns = new[]
            {
            @"<script[^>]*>.*?</script>",
            @"javascript:",
            @"on\w+\s*=",
            @"<iframe"
        };

            var allPatterns = sqlPatterns.Concat(xssPatterns);

            foreach (var pattern in allPatterns)
            {
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
