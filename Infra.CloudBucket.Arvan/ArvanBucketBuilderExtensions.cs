using System;
using Infra.Shared.CloudBucket.CloudBucketBuilders;

namespace Core.CloudBucket.Arvan
{
    public static class ArvanBucketBuilderExtensions
    {
        public static IBucketConfigurationBuilder<ArvanCloudBucket> AddArvan(this ICloudBucketBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder
                .AddBucket<ArvanCloudBucket>();
        }
    }
}
