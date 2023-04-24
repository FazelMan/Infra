using Infra.Shared.CloudBucket.Abstraction;
using Infra.Shared.CloudBucket.CloudBucketBuilders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infra.Shared.CloudBucket.Internal
{
    /// <inheritdoc />
    internal class BucketBuilder : ICloudBucketBuilder
    {
        public BucketBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public IBucketConfigurationBuilder<TCloudBucket> AddBucket<TCloudBucket>()
            where TCloudBucket : class, ICloudBucket
        {
            Services.TryAddTransient<ICloudBucket, TCloudBucket>();

            return new BucketConfigurationBuilder<TCloudBucket>(Services);
        }
    }
}
