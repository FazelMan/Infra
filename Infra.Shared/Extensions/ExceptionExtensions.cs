using System;
using System.Linq;

namespace Infra.Shared.Extensions
{
    public static class ExceptionExtensions
    {
        public static void ThrowFirstIfAny(this AggregateException ae)
        {
            if (ae.InnerExceptions.Any())
                throw ae.InnerExceptions[0];

	        throw ae;
        }

        public static string GetCompleteException(this Exception e)
        {
            var message = "";

            if (e != null && !string.IsNullOrWhiteSpace(e.Message))
                message += e.Message;

            if (e.InnerException != null && !string.IsNullOrWhiteSpace(e.InnerException.Message))
                message += $"| InnerException : {e.InnerException.Message}";

            return message;
        }
    }
}
