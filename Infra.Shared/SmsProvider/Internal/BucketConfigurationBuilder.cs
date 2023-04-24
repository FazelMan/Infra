using Infra.Shared.SmsProvider.Abstraction;
using Infra.Shared.SmsProvider.SmsBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Shared.SmsProvider.Internal
{
    internal class SmsConfigurationBuilder<TSms> : ISmsConfigurationBuilder<TSms>
        where TSms : class, ISmsService
    {
        public SmsConfigurationBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
