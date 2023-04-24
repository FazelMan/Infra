using System;
using Infra.Shared.Properties;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public class ServiceNotInitializedException : Exception
    {
        [JsonConstructor]
        public ServiceNotInitializedException() : base(Resources.ServiceNotInitialized)
        {
        }
    }
}
