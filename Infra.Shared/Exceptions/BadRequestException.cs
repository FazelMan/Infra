using System;
using System.Collections.Generic;
using Infra.Shared.Dtos.Shared;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public class BadRequestException : CustomException
    {
        public object ResultData { get; }
        public int Code { get; set; }

        [JsonConstructor]
        public BadRequestException(string message) : base(message)
        {

        }
        public BadRequestException(string message, int errorCode) : base(message)
        {
            Code = errorCode;
        }

        public BadRequestException(ErrorDto error) : base(error.Message)
        {
            Code = error.Code;
        }

        public BadRequestException(string message, object errors) : this(message)
        {
            ResultData = errors;
        }

        public class ValidationError
        {
            public string Target { get; set; }

            public List<ValidationErrorDetail> Errors { get; set; }
        }

        public class ValidationErrorDetail
        {
            public int ErrorCode { get; set; }
            public string DisplayErrorMessage { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
