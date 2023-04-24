using Microsoft.Extensions.DependencyInjection;

namespace Infra.Shared.Builder
{
    /// <summary>
    /// A builder for building the Core services.
    /// </summary>
    public interface ICoreBuilder
    {
        /// <summary>
        /// Specifies the contract for a collection of service descriptors.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
