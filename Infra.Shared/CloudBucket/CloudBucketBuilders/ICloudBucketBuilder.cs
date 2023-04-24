using Infra.Shared.CloudBucket.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Shared.CloudBucket.CloudBucketBuilders
{
    public interface ICloudBucketBuilder
    {
        IServiceCollection Services { get; }

        IBucketConfigurationBuilder<TBucket> AddBucket<TBucket>()
            where TBucket : class, ICloudBucket;
    }
}
