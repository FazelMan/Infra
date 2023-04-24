using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Infra.Shared.Extensions
{
    public static class EnvironmentExtensions
    {
        public static bool IsCI(this IWebHostEnvironment env, IHostEnvironment hostEnvironment)
        {
            if (env == null)
                throw new ArgumentNullException(nameof(env));
            return HostEnvironmentEnvExtensions.IsEnvironment(hostEnvironment, SharedConstants.CoreEnvironmentName.CI);
        }

        public static bool IsTesting(this IWebHostEnvironment env, IHostEnvironment hostEnvironment)
        {
            if (env == null)
                throw new ArgumentNullException(nameof(env));
            return HostEnvironmentEnvExtensions.IsEnvironment(hostEnvironment, SharedConstants.CoreEnvironmentName.Testing);
        }

        public static bool IsLocal(this IWebHostEnvironment env, IHostEnvironment hostEnvironment)
        {
            if (env == null)
                throw new ArgumentNullException(nameof(env));
            return HostEnvironmentEnvExtensions.IsEnvironment(hostEnvironment, SharedConstants.CoreEnvironmentName.Local);
        }

        public static bool IsTestModeEnabled(this IWebHostEnvironment env, IHostEnvironment hostEnvironment)
        {
            return (IsTesting(env, hostEnvironment) || IsCI(env, hostEnvironment) || IsLocal(env, hostEnvironment));
        }

    }
}

