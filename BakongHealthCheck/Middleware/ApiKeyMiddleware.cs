using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BakongHealthCheck.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY = "APIKey";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.TryGetValue(APIKEY, out
               var extractedApiKey))
            {
                httpContext.Response.StatusCode = 4001;
                await httpContext.Response.WriteAsync("Api Key was not provided ");
                return;
            }

            var appSettings = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEY);
            if (!apiKey.Equals(extractedApiKey))
            {
                httpContext.Response.StatusCode = 4001;
                await httpContext.Response.WriteAsync("Unauthorized client");
                return;
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
