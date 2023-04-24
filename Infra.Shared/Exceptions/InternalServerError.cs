using System;
using Infra.Shared.Dtos.Shared;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public class InternalServerError : Exception
    {
        public int Code { get; set; }

        [JsonConstructor]
        public InternalServerError(string message) : base(message)
        {
        } 
        
        public InternalServerError(ErrorDto error) : base(error.Message)
        {
            Code = error.Code;
        }
    }
}