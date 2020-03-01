using System;  
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Empty
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Empty");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();

            // intercept each request
            //app.Use(async (context, next) => {
            //    var timer = Stopwatch.StartNew();
            //    logger.LogInformation("===> I'm inside beggining app.use method");
            //    await next();
            //    logger.LogInformation($"===> I'm inside the end of app.use method {timer.ElapsedMilliseconds} ms");
            //});

            app.UseEnvironmentMiddleware();

            app.Map("/Contacts", a => a.Run(async (context) =>
            {
                logger.LogInformation("===> I'm inside the app.Map method");
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("this is your content page");
            }));

            // Identify the browser
            app.MapWhen((context) => context.Request.Headers["User-Agent"].First().Contains("OPR"), OperaRoute);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                logger.LogInformation("===> I'm inside app.UseEndpoints method");
                endpoints.MapGet("/", async context =>
                {
                    logger.LogInformation("===> I'm inside app.UseEndpoints.Mapget method");
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            app.Run(async (context) => {
                logger.LogInformation($"===> this is Run method");
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("Hello World from run");
            });
        }

        private void OperaRoute(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("Hello from Opera");
            });
        }
    }
}
