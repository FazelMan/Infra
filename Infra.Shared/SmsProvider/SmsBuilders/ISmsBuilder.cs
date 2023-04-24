using Infra.Shared.SmsProvider.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Shared.SmsProvider.SmsBuilders
{
    public interface ISmsBuilder
    {
        IServiceCollection Services { get; }

        ISmsConfigurationBuilder<TSms> AddSmsProvider<TSms>()
            where TSms : class, ISmsService;
    }
}
