using System;

namespace Infra.Shared.Helpers
{
    public static class DateTimeExtensions
    {
        public static string ElapsedTime(this DateTime dateTime)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            var ts = new TimeSpan(DateTime.Now.Ticks - dateTime.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);
            
            if (delta < 1 * minute)
                return ts.Seconds == 1 ? "لحظه ای قبل" : ts.Seconds + " ثانیه قبل";
            
            if (delta < 2 * minute)
                return "یک دقیقه قبل";
            
            if (delta < 45 * minute)
                return ts.Minutes + " دقیقه قبل";
            
            if (delta < 90 * minute)
                return "یک ساعت قبل";
            
            if (delta < 24 * hour)
                return ts.Hours + " ساعت قبل";
            
            if (delta < 48 * hour)
                return "دیروز";
            
            if (delta < 30 * day)
                return ts.Days + " روز قبل";
            
            if (delta < 12 * month)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "یک ماه قبل" : months + " ماه قبل";
            }

            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "یک سال قبل" : years + " سال قبل";
        }
    }
}