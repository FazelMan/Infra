using System;
using System.Collections.Generic;

namespace Infra.Shared.Helpers
{
    public static class DateTimeHelper
    {
        public static PersianDateTime ToPersian(this DateTime date)
        {
            var calendar = new System.Globalization.PersianCalendar();

            var result = new PersianDateTime()
            {
                Day = calendar.GetDayOfMonth(date),
                Month = calendar.GetMonth(date),
                Year = calendar.GetYear(date),
                Hour = calendar.GetHour(date),
                Minute = calendar.GetMinute(date),
                Seconds = calendar.GetSecond(date),
                Milliseconds = calendar.GetMilliseconds(date)
            };

            return result;
        }
        /// <summary>
        /// if -1 date1 greather than date2
        /// if 0 is equal
        /// if 1 date2 greather than date1
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static int CompareDates(DateTime date1, DateTime date2)
        {
            return DateTime.Compare(date1, date2);
        }
        public static int GetDayCount(string unit, int value)
        {
            switch (unit.ToLower())
            {
                case "year":
                    return (int)Math.Round(value * 365.2);
                case "month":
                    return (int)Math.Round(value * 30.5);
                case "day":
                    return value;
                default:
                    return 0;
            }
        }

        public static string GetShortDateEn(DateTime? date)
        {
            if (date == null)
                return "---";

            return date.Value.ToShortDateString();
        }

        private static string GetMonthsTitle(byte input)
        {
            string[] arr = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
            return arr[input - 1];
        }

        public static string GetMonthsFromCode(int input)
        {
            var partY = input.ToString().Substring(0, 4);
            var partM = input.ToString().Substring(4, 2);

            return GetMonthsTitle(byte.Parse(partM)) + " ماه " + partY;
        }

        public static Dictionary<string, string> GetMonthYearFromCurrrentYear(byte id_Lang)
        {
            var calendar = new System.Globalization.PersianCalendar();

            int currentYear = calendar.GetYear(DateTime.Now);
            int currentMonth = calendar.GetMonth(DateTime.Now);

            var list = new Dictionary<string, string>();

            for (int i = currentYear; i >= 1397; i--)
            {
                for (int j = 12; j >= 1; j--)
                {
                    if ((i < currentYear) || (i == currentYear && j <= currentMonth))
                        list.Add(string.Format("{0}/{1}/{2}", i, j.ToString().PadLeft(2, '0'), "31"), string.Format("{0} {1}", Enum.GetName(typeof(PersianMonths), j), i));
                }
            }

            return list;
        }

        public static Dictionary<string, string> GetGregorianMonthYearFromYear(byte id_Lang, short startYear)
        {
            var calendar = new System.Globalization.PersianCalendar();

            int currentMonth = DateTime.Now.Month;

            var list = new Dictionary<string, string>();

            for (int i = DateTime.Now.Year; i >= startYear; i--)
            {

                if (i < DateTime.Now.Year)
                {
                    list.Add(i.ToString(), i.ToString());
                }

                for (int j = 12; j >= 1; j--)
                {
                    //if ((i >= startYear) && (j <= currentMonth))
                    if ((i == DateTime.Now.Year && j <= currentMonth) || i < DateTime.Now.Year)
                    {
                        var _date = new DateTime(i, j, 1).AddMonths(1).AddDays(-1);
                        list.Add(_date.ToShortDateString(), string.Format("{0} {1}", Enum.GetName(typeof(GregorianMonths), j), i));
                    }
                }
            }

            return list;
        }
    }
}
