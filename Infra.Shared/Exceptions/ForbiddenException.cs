using System;
using Infra.Shared.Dtos.Shared;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public class ForbiddenException : CustomException
    {
        public int Code { get; set; }

        [JsonConstructor]
        public ForbiddenException(string message) : base(message)
        {
        }

        public ForbiddenException(ErrorDto error) : base(error.Message)
        {
            Code = error.Code;
        }
    }
}