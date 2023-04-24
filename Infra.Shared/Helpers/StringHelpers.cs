using System;

namespace Infra.Shared.Helpers
{
    public class StringHelpers
    {

        public static string FormatMoney(decimal value)
        {
            var result = ((double)value).ToString("N").Replace("/", ".");
            result = hasDecimal(result) == false ? result.Substring(0, result.IndexOf(".")) : result;
            return result;
        }

        public static string FormatNumber(decimal value)
        {
            var result = ((double)value).ToString().Replace("/", ".");
            result = hasDecimal(result) ? result :
                result.IndexOf(".") >= 0 ? result.Substring(0, result.IndexOf(".")) : result;
            return result;
        }

        private static bool hasDecimal(string value)
        {
            if (value.IndexOf('.') < 0) return false;
            var decimalValue = value.Substring(value.IndexOf('.') + 1);
            return Convert.ToDouble(decimalValue) > 0;
        }

    }
}
