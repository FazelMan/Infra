using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Infra.Shared.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Infra.Shared.Http.Extensions;
using Infra.Shared.Exceptions;
using Infra.Shared.Helpers;
using Infra.Shared.Http.Filters;
using Infra.Shared.Services.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Host = Infra.Shared.Helpers.Host;

namespace Infra.Shared.Service
{
    public static class ServiceRegistrar
    {
        public static void RegisterMvcServices(
            this IServiceCollection services,
            IWebHostEnvironment env,
            Action<Options> action)
        {
            if (action == null)
                throw new ArgumentException();

            var options = new Options();

            action(options);

            services.AddLogging(b =>
            {
                b.AddConsole(c => c.IncludeScopes = true);

                b.AddDebug();
            });

            AddApiVersioning(services);

            services.AddCors();

            services.AddSignalR().AddHubOptions<LiveDataHub>(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
            });

            services.AddTransient<ISignalRSender, SignalRSender>();

            if (options.RouteConfig != null)
            {
                services
                    .AddMvc(opt =>
                    {
                        opt.UseCentralRoutePrefix(new RouteAttribute(options.RouteConfig.Prefix));
                        // opt.Filters.Add(typeof(ModelValidationTransformation));
                        opt.Filters.Add(typeof(TtfbCalculator));
                    })
                    .AddMvcOptions(opt =>
                    {
                        // opt.Filters.Add<ExceptionActionFilter>();
                        // opt.Filters.Add(typeof(AutomaticModelStateValidatorAttribute));
                    });
            }

            if (options.SwaggerConfig != null)
            {
                services.AddSwaggerGen(opt =>
                {
                    if (options.SwaggerConfig.AuthorizationHeaderRequired)
                    {
                        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            Description =
                                "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey,
                            Scheme = "Bearer"
                        });

                        opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    },
                                    Scheme = "oauth2",
                                    Name = "Bearer",
                                    In = ParameterLocation.Header,
                                },
                                new List<string>()
                            }
                        });
                    }

                    opt.SwaggerDoc(
                        options.SwaggerConfig.Version,
                        new OpenApiInfo
                        {
                            Version = options.SwaggerConfig.Version,
                            Title = options.SwaggerConfig.Title
                        });

                    if (!string.IsNullOrWhiteSpace(options.SwaggerConfig.XmlDocPath))
                        opt.IncludeXmlComments(options.SwaggerConfig.XmlDocPath);
                });
            }

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = Host.Config
                    .GetSection("SupportedCultures")
                    .Get<CultureInfo[]>();

                options.DefaultRequestCulture = new RequestCulture(culture: "fa", uiCulture: "fa");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
            });

            services.AddHealthChecks();
        }

        private static void AddApiVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(
                options => { options.ReportApiVersions = true; });

            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });
        }

        public class Options
        {
            public SwaggerConfigInfo SwaggerConfig { get; set; }
            public RouteConfigInfo RouteConfig { get; set; }

            public class SwaggerConfigInfo
            {
                public string Version { get; set; }
                public string Title { get; set; }
                public bool AuthorizationHeaderRequired { get; set; }
                public string XmlDocPath { get; set; }
            }

            public class RouteConfigInfo
            {
                public string Prefix { get; set; }
            }
        }
    }
}