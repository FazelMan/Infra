using Infra.Shared.SmsProvider.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Shared.SmsProvider.SmsBuilders
{
    public interface ISmsConfigurationBuilder<TSms> where TSms : class, ISmsService
    {
        IServiceCollection Services { get; }
    }
}
