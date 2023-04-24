using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Infra.Shared.Helpers
{
    public static class Host
    {
        public static IConfiguration Config { get; set; }

        public static void Init()
        {
            var secretFile = Environment.GetEnvironmentVariable("SECRET_FILE");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
                .AddJsonFile($"appsettings.local.json", true, true)
                .AddEnvironmentVariables();
            
            if (secretFile != null)
                builder.AddJsonFile($"{secretFile}", true, true);

            Config = builder.Build();
        }
    }
}