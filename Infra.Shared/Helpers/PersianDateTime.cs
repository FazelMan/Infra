using System;
using System.Collections.Generic;

namespace Infra.Shared.Helpers
{
    public class PersianDateTime
    {
        #region Properties
        private DateTime _GeorgianDate;

        public DateTime GeorgianDate
        {
            set
            {
                _GeorgianDate = value;
            }
            get
            {
                return this._GeorgianDate;
            }

        }

        public int Day
        {
            get;
            set;
        }

        public int Month
        {
            get;
            set;
        }

        public int Year
        {
            get;
            set;
        }

        public int Hour
        {
            get;
            set;
        }

        public int Minute
        {
            get;
            set;
        }

        public int Seconds
        {
            set;
            get;
        }

        public double Milliseconds
        {
            get;
            set;
        }

        #endregion

        #region Methods
        public PersianDateTime()
        {

        }

        public string GetDescriptiveDate
        {
            get
            {
                Dictionary<DayOfWeek, string> days = new Dictionary<DayOfWeek, string>();

                days.Add(DayOfWeek.Wednesday, "چهار شنبه");
                days.Add(DayOfWeek.Thursday, "پنج‌شنبه");
                days.Add(DayOfWeek.Friday, "جمعه");
                days.Add(DayOfWeek.Saturday, "شنبه");
                days.Add(DayOfWeek.Sunday, "یکشنبه");
                days.Add(DayOfWeek.Monday, "دوشنبه");
                days.Add(DayOfWeek.Tuesday, "سه‌شنبه");

                Dictionary<int, string> months = new Dictionary<int, string>();

                months.Add(1, "فروردین");
                months.Add(2, "اردیبهشت");
                months.Add(3, "خرداد");

                months.Add(4, "تیر");
                months.Add(5, "مرداد");
                months.Add(6, "شهریور");

                months.Add(7, "مهر");
                months.Add(8, "آبان");
                months.Add(9, "اذر");

                months.Add(10, "دی");
                months.Add(11, "بهمن");
                months.Add(12, "اسفند");

                System.Globalization.PersianCalendar calendar = new System.Globalization.PersianCalendar();
                string result = string.Format("{0} {1} {2} {3}", days[calendar.GetDayOfWeek(this.GeorgianDate)], this.Day, months[this.Month], this.Year);

                return result;
            }
        }

        public string GetShortDate()
        {
            return string.Format("{0}/{1}/{2}", this.Year, this.Month, this.Day);
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2} {3}:{4}:{5}", this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Seconds);

        }

        public static PersianDateTime FromDateTime(DateTime dateTime)
        {
            return dateTime.ToPersian();
        }
        public static string PersianStringDate(DateTime? dateTime)
        {
            if (dateTime == null) return "";

            PersianDateTime pDateTime = dateTime.Value.ToPersian();
            string ssDate = pDateTime.Year.ToString() + "/";
            if (pDateTime.Month < 10) ssDate += "0";
            ssDate += pDateTime.Month + "/";
            if (pDateTime.Day < 10) ssDate += '0';
            ssDate += pDateTime.Day;
            return ssDate;
        }
        public static string PersianStringDateTime(DateTime? dateTime)
        {
            if (dateTime == null) return "";

            try
            {
                PersianDateTime pDateTime = dateTime.Value.ToPersian();
                string ssDate = pDateTime.Year.ToString() + "/";
                if (pDateTime.Month < 10) ssDate += "0";
                ssDate += pDateTime.Month + "/";
                if (pDateTime.Day < 10) ssDate += '0';
                ssDate += pDateTime.Day;
                ssDate += " " + dateTime.Value.ToString("HH:mm");
                return ssDate;
            }
            catch
            {
                return "";
            }
        }
        public static PersianDateTime TodayPersian()
        {
            DateTime Today = DateTime.Now;
            return Today.ToPersian();
        }

        public static int GetAge(DateTime? BirthDate, DateTime? date_Now = null)
        {
            if (BirthDate == null) return 0;
            else
            {
                if (date_Now == null)
                    date_Now = DateTime.Now;
                TimeSpan difference = date_Now.Value - BirthDate.Value;
                return difference.Days;
            }
        }
        public static bool IsleapYear(int year)
        {
            var persianCal = new System.Globalization.PersianCalendar();
            return persianCal.IsLeapYear(year);
        }
        public static DateTime AddPersianDate(int value, string type, DateTime? date)
        {
            DateTime dt = date ?? DateTime.Now;
            if (type == "year" && value < -200) value = -200;
            var persianCal = new System.Globalization.PersianCalendar();
            if (type == "year") dt = persianCal.AddYears(dt, value);
            if (type == "month") dt = persianCal.AddMonths(dt, value);
            if (type == "day") dt = persianCal.AddDays(dt, value);
            return dt;
        }
        public static DateTime AddGeorgianDate(int value, string type, DateTime? date = null)
        {
            DateTime dt = date ?? DateTime.Now;
            if (type == "year") dt = dt.AddYears(value);
            if (type == "month") dt = dt.AddMonths(value);
            if (type == "day") dt = dt.AddDays(value);
            return dt;
        }
        public static string GetAgePersianString(string date, DateTime? date_Now)
        {
            if (date_Now == null) date_Now = System.DateTime.Now;
            int years = 0;
            int months = 0;
            int days = 0;
            if (GetAgePersian(date, out years, out months, out days, date_Now) == false) return "";
            string ss = "";
            if (years > 0) ss = years.ToString() + " سال";
            if (months > 0)
            {
                if (ss != "") ss = ss + " و ";
                ss = ss + months + " ماه";
            }
            if (days > 0)
            {
                if (ss != "") ss = ss + " و ";
                ss = ss + days + " روز";
            }
            if (ss == "") ss = "بدو تولد";
            return ss;
        }

        public static int GetAgePersianMonth(DateTime date)
        {
            int years = 0;
            int months = 0;
            int days = 0;
            GetAgePersian(date, out years, out months, out days);
            return years * 12 + months;
        }
        public static bool GetAgePersian(string date, out int years, out int months, out int days, DateTime? date_Now = null)
        {
            if (date_Now == null) date_Now = System.DateTime.Now;
            years = 0;
            months = 0;
            days = 0;
            if (PersianDateTimeHelper.CheckDateFormat(date) == false)
            {
                return false;
            }
            if (GetAge(PersianDateTimeHelper.GetGeorgianFromPersian(date) ?? DateTime.Now, date_Now) < 0) return false;
            var dateArrMin = date.Split('/');
            var dateArrMax = PersianStringDate(date_Now.Value).Split('/');
            int yearMax = Convert.ToInt16(dateArrMax[0]);
            int yearMin = Convert.ToInt16(dateArrMin[0]);
            int monthMax = Convert.ToInt16(dateArrMax[1]);
            int monthMin = Convert.ToInt16(dateArrMin[1]);
            int dayMax = Convert.ToInt16(dateArrMax[2]);
            int dayMin = Convert.ToInt16(dateArrMin[2]);
            years = yearMax - yearMin;
            if (dayMax >= dayMin)
            {
                days = dayMax - dayMin;
                months = monthMax - monthMin;
            }
            else
            {
                if (monthMax <= 7)
                {
                    days = dayMax - dayMin + 31;
                    months = monthMax - monthMin - 1;
                }
                else
                if (monthMax == 1 && IsleapYear(yearMax - 1))
                {
                    days = dayMax - dayMin + 29;
                    months = monthMax - monthMin - 1;
                }
                else
                {
                    days = dayMax - dayMin + 30;
                    months = monthMax - monthMin - 1;
                }
            }
            if (months < 0)
            {
                months = months + 12;
                years = years - 1;
            }
            return true;
        }
        public static bool GetAgePersian(DateTime date, out int years, out int months, out int days)
        {
            return GetAgePersian(PersianStringDate(date), out years, out months, out days);
        }
        public static void CalculateAge(DateTime birthDate, out int years, out int months, out int days, DateTime? date_Now = null)
        {
            float ageDay = GetAge(birthDate, date_Now);
            years = (int)(ageDay / 365.25);
            months = (int)((ageDay - (years * 365.25)) / 30.4375);
            days = (int)(ageDay - (years * 365.25) - (months * 30.4375));
        }

        public static string GetAgeStringByDay(int day)
        {
            var birthDate = DateTime.Now.AddDays((day * -1));
            return GetAgeString(birthDate);
        }

        public static string GetAgeString(DateTime birthDate, int longDate = 0, DateTime? date_Now = null)
        {
            int years = 0;
            int months = 0;
            int days = 0;
            CalculateAge(birthDate, out years, out months, out days, date_Now);
            string ss = "";
            if (years > 0)
            {
                ss += string.Format("{0} سال", years);
            }
            if (months > 0)
                if (longDate == 1 || years < 10)
                {
                    if (ss != "") ss += " و ";
                    ss += string.Format("{0}ماه ", months);
                }
            if (days > 0)
                if (longDate == 1 || years < 1)
                {
                    if (ss != "") ss += " و ";
                    ss += string.Format("{0} روز", days);
                }
            return ss;
        }

        public static int GetPersianYear(DateTime dateTime)
        {
            PersianDateTime pDateTime = dateTime.ToPersian();
            return pDateTime.Year;
        }
        #endregion
    }
}
