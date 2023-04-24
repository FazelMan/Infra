using System;
using Infra.Shared.Enums;
using Newtonsoft.Json;

namespace Infra.Shared.Exceptions
{
    public static class UnexpectedResultException
    {
        public static void CheckUnexpectedResult(int number)
        {
            if (number < 0)
            {
                var error = Math.Abs(number).GetErrorMessage();
                throw new BadRequestException(error.Message, number);
            }

            if (number == 0)
            {
                var error = ErrorCodeType.UnexpectedResult.GetErrorMessage();
                throw new BadRequestException(error.Message, error.Code);
            }
        }
    }
}