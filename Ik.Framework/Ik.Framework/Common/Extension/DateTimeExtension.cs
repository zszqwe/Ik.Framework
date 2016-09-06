using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Extension
{
    public static class DateTimeExtensions
    {
        public const string COMMON_DATETIME = "yyyy-MM-dd HH:mm:ss";

        public const string COMMON_DATE = "yyyy-MM-dd";

        public static string ToTimePeriodDescription(this DateTime dt)
        {
            var str = "";
            var timespan = DateTime.Now - dt;
            var days = timespan.TotalDays;
            var hours = timespan.TotalHours;
            var mins = timespan.TotalMinutes;
            if (days > 1)
                str = (int)days + "天前";
            else if (hours > 1)
                str = (int)hours + "小时前";
            else
                str = (int)mins + "分钟前";
            return str;
        }

        public static DateTime GetMonthStart(this DateTime dt)
        {
            return dt.Date.AddDays(-dt.Day + 1);
        }

        //TODO: 把从星期几开始设为参数，可以实现任意时间开始
        public static DateTime GetWeekStart(this DateTime dt, bool startFromMonday = true)
        {
            var dayToReduce = startFromMonday ? -(((int)dt.DayOfWeek + 6) % 7) : -(int)dt.DayOfWeek;
            return dt.Date.AddDays(dayToReduce);
        }

        //两个时间之间相关多少个月。注意：取的是月份，比如2014-10-31和2014-11-01相差是一个月
        public static int MonthDiff(this DateTime dt1, DateTime dt2)
        {
            var month = (dt1.Year - dt2.Year) * 12 + dt1.Month - dt2.Month;
            return month;
        }

        public static long GetJsTimespan(this DateTime dt)
        {
            return (long)((dt - new DateTime(1970, 1, 1)).TotalMilliseconds);

        }

        public static string ToCommonDateTimeString(this DateTime dt)
        {
            return dt.ToString(COMMON_DATETIME);
        }

        public static string ToCommonDateTimeString(this DateTime? dt)
        {

            return dt.HasValue ? dt.Value.ToString(COMMON_DATETIME) : "";
        }

        public static string ToCommonDateString(this DateTime dt)
        {
            return dt.ToString(COMMON_DATE);
        }

        public static string ToCommonDateString(this DateTime? dt)
        {
            return dt.HasValue ? dt.Value.ToString(COMMON_DATE) : "";
        }

        public static bool IsToday(this DateTime time)
        {
            DateTime now = DateTime.Now;
            return time.Year == now.Year && time.Month == now.Month && time.Day == now.Day;
        }

        public static bool IsMinValue1970(this DateTime time)
        {
            return time.Year == 1970 && time.Month == 1 && time.Day == 1;
        }
    }
}

#region copyright
/*
*.NET基础开发框架
*Copyright (C) 。。。
*地址：git@github.com:gangzaicd/Ik.Framework.git
*作者：到大叔碗里来（大叔）
*QQ：397754531
*eMail：gangzaicd@163.com
*/
#endregion copyright
