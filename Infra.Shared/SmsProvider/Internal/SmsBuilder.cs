using Infra.Shared.CloudBucket.Abstraction;
using Infra.Shared.SmsProvider.Abstraction;
using Infra.Shared.SmsProvider.SmsBuilders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infra.Shared.SmsProvider.Internal
{
    /// <inheritdoc />
    internal class SmsBuilder : ISmsBuilder
    {
        public SmsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public ISmsConfigurationBuilder<TSms> AddSmsProvider<TSms>()
            where TSms : class, ISmsService
        {
            Services.TryAddTransient<ISmsService, TSms>();

            return new SmsConfigurationBuilder<TSms>(Services);
        }
    }
}
