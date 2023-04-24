using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Infra.Shared.Dtos.Shared;
using Infra.Shared.Enums;
using Infra.Shared.Properties;

namespace Infra.Shared.Exceptions
{
    public static class UserFriendlyErrorMessages
    {
        public static ErrorDto GetErrorMessage(this int errorCode)
        {
            var error = Enum.GetName(typeof(ErrorCodeType), (ErrorCodeType)errorCode);
            var resourceManager = new ResourceManager(typeof(Resources));

            return new ErrorDto
            {
                Code = errorCode,
                Message = resourceManager.GetString(error)
            };
        }

        public static ErrorDto GetErrorMessage(this ErrorCodeType errorCode)
        {
            var error = Enum.GetName(typeof(ErrorCodeType), errorCode);
            var resourceManager = new ResourceManager(typeof(Resources));

            return new ErrorDto
            {
                Code = (int)errorCode,
                Message = resourceManager.GetString(error)
            };
        }
    }
}