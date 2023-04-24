using System;
using Infra.Shared.Dtos.Shared;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public class CustomException : Exception
    {
        public int Code { get; set; }

        [JsonConstructor]
        public CustomException(string message) : base(message)
        {
        } 
        
        public CustomException(ErrorDto error) : base(error.Message)
        {
            Code = error.Code;
        }
    }
}