using System;
using Infra.Shared.Properties;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public class NotFoundException : Exception
    {
        [JsonConstructor]
        public NotFoundException() : base(Resources.DataNotFound)
        {
        }
    }
}