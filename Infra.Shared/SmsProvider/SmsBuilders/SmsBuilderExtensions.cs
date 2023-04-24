using System;
using Infra.Shared.Builder;
using Infra.Shared.SmsProvider.Internal;

namespace Infra.Shared.SmsProvider.SmsBuilders
{
    public static class SmsBuilderExtensions
    {
        public static ICoreBuilder ConfigureSmsProviders(this ICoreBuilder builder, Action<ISmsBuilder> configure)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            configure(new SmsBuilder(builder.Services));

            return builder;
        }
    }
}
