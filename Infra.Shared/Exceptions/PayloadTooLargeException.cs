using System;
using Infra.Shared.Dtos.Shared;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public class PayloadTooLargeException : CustomException
    {
        public int Code { get; set; }

        [JsonConstructor]
        public PayloadTooLargeException(string message) : base(message)
        {
        } 
        
        public PayloadTooLargeException(ErrorDto error) : base(error.Message)
        {
            Code = error.Code;
        }
    }
}