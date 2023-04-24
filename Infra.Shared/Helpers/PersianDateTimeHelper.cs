using System;
using System.Collections.Generic;
using System.Globalization;

namespace Infra.Shared.Helpers
{
    public static class PersianDateTimeHelper
    {
        private static Dictionary<int, string> _monthsDictionary = new Dictionary<int, string>();

        static PersianDateTimeHelper()
        {

            foreach (int value in Enum.GetValues(typeof(PersianMonths)))
            {
                _monthsDictionary.Add(value, ((PersianMonths)value).ToString());
            }
        }

        public static bool IsDateValid(string date)
        {
            try
            {
                if (GetGeorgianFromPersian(date).HasValue)
                    return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsDateInFeature(string date)
        {
            try
            {
                var _date = GetGeorgianFromPersian(date);
                if (_date.HasValue && _date <= DateTime.Now)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }


        public static DateTime GetGeorgianFromPersian(int year, int month, int day)
        {
            var calendar = new PersianCalendar();

            var date = calendar.ToDateTime(year, month, day, 0, 0, 0, 0);

            return date;
        }

        public static DateTime? GetGeorgianFromPersian(string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;

            DateTime? retVal;

            try
            {
                var parts = new string[3];

                if (date.Contains("/"))
                {
                    parts = date.Split('/');
                }
                else
                {
                    parts[0] = date.Substring(0, 4);
                    parts[1] = date.Substring(4, 2);
                    parts[2] = date.Substring(6, 2);
                }

                if (parts.Length < 3)
                    return null;

                int year,
                    month,
                    day;

                if (int.TryParse(parts[0], out year) && int.TryParse(parts[1], out month) && int.TryParse(parts[2], out day))
                    retVal = GetGeorgianFromPersian(year, month, day);
                else
                    retVal = null;
            }
            catch (Exception ex)
            {
                retVal = null;
            }

            return retVal;
        }

        public static string GetMonthName(this PersianDateTime persianDate)
        {
            return _monthsDictionary[persianDate.Month];
        }

        public static string GetRegularDate(this PersianDateTime perdianDate, string mode = "/")
        {
            var day = "00" + perdianDate.Day.ToString();
            var month = "00" + perdianDate.Month.ToString();
            return perdianDate.Year.ToString() + mode + month.Substring(month.Length - 2) + mode + day.Substring(day.Length - 2);
        }
        public static bool CheckDateFormat(string date)
        {
            var dateArr = date.Split('/');
            if (dateArr.Length == 3 && dateArr[0].ToString().Length == 4 && dateArr[1].ToString().Length == 2 && dateArr[2].ToString().Length == 2)
                return true;
            return false;
        }


    }

    public enum PersianMonths
    {
        فروردین = 1,
        اردیبهشت = 2,
        خرداد = 3,
        تیر = 4,
        مرداد = 5,
        شهریور = 6,
        مهر = 7,
        آبان = 8,
        آذر = 9,
        دی = 10,
        بهمن = 11,
        اسفند = 12
    }

    public enum GregorianMonths
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

}
