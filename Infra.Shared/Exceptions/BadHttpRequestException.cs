using System;
using Infra.Shared.Dtos.Shared;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public class BadHttpRequestException : CustomException
    {
        public int Code { get; set; }

        [JsonConstructor]
        public BadHttpRequestException(string message) : base(message)
        {
        } 
        
        public BadHttpRequestException(ErrorDto error) : base(error.Message)
        {
            Code = error.Code;
        }
    }
}