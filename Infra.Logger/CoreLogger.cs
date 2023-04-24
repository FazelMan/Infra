using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Infra.Logger
{
    public static class CoreLogger
    {
        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string indexName,
            string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearchSecret:Uri"]))
            {
                ModifyConnectionSettings =
                    x => x.BasicAuthentication(configuration["ElasticSearchSecret:Username"],
                        configuration["ElasticSearchSecret:Password"]),
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                IndexFormat =
                    $"{indexName?.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-logs"
            };
        }

        public static void ConfigureLogging(string indexName)
        {
            var environment =
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSink((IConfigurationRoot)Infra.Shared.Helpers.Host.Config, indexName, environment))
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration((IConfigurationRoot)Infra.Shared.Helpers.Host.Config)
                .CreateLogger();
        }
    }
}