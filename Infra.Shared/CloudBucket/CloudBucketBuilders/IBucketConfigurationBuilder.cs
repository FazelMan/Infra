using Infra.Shared.CloudBucket.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Shared.CloudBucket.CloudBucketBuilders
{
    public interface IBucketConfigurationBuilder<TBucket> where TBucket : class, ICloudBucket
    {
        IServiceCollection Services { get; }
    }
}
