using System;
using Infra.Shared.Configuration;
using Infra.Shared.SmsProvider.SmsBuilders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.SmsProvider.Rahyab
{
    public static class RahyabBuilderExtensions
    {
        public static ISmsConfigurationBuilder<RahyabProvider> AddRahyab(this ISmsBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            
            var smsConfiguration = Infra.Shared.Helpers.Host.Config.GetSection(nameof(SmsConfiguration)).Get<SmsConfiguration>();
            builder.Services.AddSingleton(smsConfiguration);
            
            return builder
                .AddSmsProvider<RahyabProvider>();
        }
    }
}
