using System;
using System.Threading.Tasks;
using Infra.Shared.Http.Middleware;
using Infra.Shared.Services.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Hosting;

namespace Infra.Shared.Service
{
    public static class ServiceConfigurator
    {
        public static void ConfigureMvcMiddlewares(
            this IApplicationBuilder app,
            IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider,
            Action<Options> action,
            Func<Microsoft.AspNetCore.Http.HttpContext, Func<Task>, Task> onRoutingConfigured = null)
        {
            if (action == null)
                throw new ArgumentException();

            var options = new Options();

            action(options);

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseRequestLocalization();

            app.UseMiddleware(typeof(MemoryMonitoringMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<LiveDataHub>("/api/live-data");
            });

            if (options.SwaggerConfig != null)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        c.SwaggerEndpoint(options.SwaggerConfig.Endpoint,
                            description.GroupName.ToUpperInvariant());
                    }

                    c.OAuthClientId(options.SwaggerConfig.OAuthClientId);
                    c.OAuthAppName($"{options.SwaggerConfig.Title} - Swagger");
                });
            }

            if (onRoutingConfigured != null)
                app.Use(onRoutingConfigured);
        }

        public class Options
        {
            public LoggingInfo Logging { get; set; }
            public SwaggerConfigInfo SwaggerConfig { get; set; }

            public class LoggingInfo
            {
                public string FilePath { get; set; }
            }

            public class SwaggerConfigInfo
            {
                public string OAuthClientId { get; set; }
                public string Endpoint { get; set; }
                public string Title { get; set; }
            }
        }
    }
}