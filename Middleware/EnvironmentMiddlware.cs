using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Empty
{
    internal class EnvironmentMiddlware
    {
        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment environment;
        private readonly ILogger logger;

        public EnvironmentMiddlware(RequestDelegate next, IWebHostEnvironment environment, ILoggerFactory logger)
        {
            this.next = next;
            this.environment = environment;
            this.logger = logger.CreateLogger("Enviornment");
        }

        public  async Task Invoke(HttpContext context)
        {
            var timer = Stopwatch.StartNew();
            logger.LogInformation("===> I'm inside beggining Invoke method");

            context.Response.Headers.Add("X-HostingEnvironmentType", new[] { environment.EnvironmentName });

            await next(context);
            
            if (environment.IsDevelopment() &&
                context.Response.ContentType != null &&
                context.Response.ContentType == "text/html")
            {
                await context.Response.WriteAsync($"<p> This is coming from development Environment  {timer.ElapsedMilliseconds}</p>");
            }

            logger.LogInformation($"===> I'm inside the end of Invoke method {timer.ElapsedMilliseconds} ms");
        }

    }

    public static class MiddlewareHelpers
    {
        public static IApplicationBuilder UseEnvironmentMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<EnvironmentMiddlware>();
        }
    }
}