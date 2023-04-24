using System;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public class InternalServerException : CustomException
    {
        public new string StackTrace { get; }

        public InternalServerException(string message) : base(message)
        {
        }

        [JsonConstructor]
        public InternalServerException(string message, string trace) : this(message)
        {
            StackTrace = trace;
        }
    }
}
