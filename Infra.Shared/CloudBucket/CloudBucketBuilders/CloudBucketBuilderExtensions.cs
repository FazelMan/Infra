using System;
using Infra.Shared.Builder;
using Infra.Shared.CloudBucket.Internal;

namespace Infra.Shared.CloudBucket.CloudBucketBuilders
{
    public static class CloudBucketBuilderExtensions
    {
        public static ICoreBuilder ConfigureBuckets(this ICoreBuilder builder, Action<ICloudBucketBuilder> configure)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            configure(new BucketBuilder(builder.Services));

            return builder;
        }
    }
}
