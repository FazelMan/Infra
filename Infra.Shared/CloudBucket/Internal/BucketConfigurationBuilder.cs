using Infra.Shared.CloudBucket.Abstraction;
using Infra.Shared.CloudBucket.CloudBucketBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Shared.CloudBucket.Internal
{
    internal class BucketConfigurationBuilder<TCloudBucket> : IBucketConfigurationBuilder<TCloudBucket>
        where TCloudBucket : class, ICloudBucket
    {
        public BucketConfigurationBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
