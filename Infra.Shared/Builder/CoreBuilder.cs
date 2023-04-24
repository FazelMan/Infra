using System;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Shared.Builder
{
    /// <inheritdoc />
    public class CoreBuilder : ICoreBuilder
    {
        public CoreBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        public static ICoreBuilder CreateDefaultBuilder(IServiceCollection services = null)
        {
            services ??= new ServiceCollection();

            var builder = new CoreBuilder(services);

            builder.Services.AddOptions();

            return builder;
        }
    }
}
