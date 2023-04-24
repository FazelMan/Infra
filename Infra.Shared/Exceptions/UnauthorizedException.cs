using System;
using Infra.Shared.Dtos.Shared;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public class UnauthorizedException : CustomException
    {
        public int Code { get; set; }

        [JsonConstructor]
        public UnauthorizedException(string message) : base(message)
        {
        } 
        
        public UnauthorizedException(ErrorDto error) : base(error.Message)
        {
            Code = error.Code;
        }
    }
}
