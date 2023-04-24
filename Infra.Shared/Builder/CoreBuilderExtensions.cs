using System;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Shared.Builder
{
    public static class CoreBuilderExtensions
    {
        public static ICoreBuilder AddCore(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            return CoreBuilder.CreateDefaultBuilder(services);
        }
    }
}
