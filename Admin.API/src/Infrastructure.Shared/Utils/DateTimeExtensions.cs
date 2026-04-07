/*
 * 文件名称: DateTimeExtensions.cs
 * 功能描述: 日期时间扩展方法，提供常用的日期时间操作
 * 作者信息: 谢灿软件 <492384481@qq.com>
 * 最近修订: 2026-04-05
 */

namespace Infrastructure.Shared.Utils;

/// <summary>
/// 日期时间扩展方法
/// <para>提供常用的日期时间操作，包括格式化、计算、时区转换等</para>
/// </summary>
/// <example>
/// <code>
/// var now = DateTime.Now;
/// 
/// // 格式化
/// var dateStr = now.ToDateString(); // "2026-04-06"
/// var timeStr = now.ToTimeString(); // "14:30:25"
/// var dateTimeStr = now.ToDateTimeString(); // "2026-04-06 14:30:25"
/// 
/// // 时间范围
/// var start = now.StartOfDay(); // 当天 00:00:00
/// var end = now.EndOfDay(); // 当天 23:59:59.9999999
/// 
/// // 时间戳转换
/// var timestamp = now.ToTimestamp(); // Unix 时间戳（秒）
/// var fromTimestamp = DateTimeExtensions.FromTimestamp(timestamp);
/// </code>
/// </example>
public static class DateTimeExtensions {
    /// <summary>
    /// 格式化为标准日期字符串
    /// <para>格式：yyyy-MM-dd</para>
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>格式化后的日期字符串</returns>
    /// <example>
    /// <code>
    /// var date = new DateTime(2026, 4, 6, 14, 30, 25);
    /// var result = date.ToDateString(); // 返回："2026-04-06"
    /// </code>
    /// </example>
    public static string ToDateString(this DateTime dateTime) {
        return dateTime.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// 格式化为标准时间字符串
    /// <para>格式：HH:mm:ss</para>
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>格式化后的时间字符串</returns>
    /// <example>
    /// <code>
    /// var date = new DateTime(2026, 4, 6, 14, 30, 25);
    /// var result = date.ToTimeString(); // 返回："14:30:25"
    /// </code>
    /// </example>
    public static string ToTimeString(this DateTime dateTime) {
        return dateTime.ToString("HH:mm:ss");
    }

    /// <summary>
    /// 格式化为标准日期时间字符串
    /// <para>格式：yyyy-MM-dd HH:mm:ss</para>
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>格式化后的日期时间字符串</returns>
    /// <example>
    /// <code>
    /// var date = new DateTime(2026, 4, 6, 14, 30, 25);
    /// var result = date.ToDateTimeString(); // 返回："2026-04-06 14:30:25"
    /// </code>
    /// </example>
    public static string ToDateTimeString(this DateTime dateTime) {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 格式化为标准日期时间字符串（带毫秒）
    /// <para>格式：yyyy-MM-dd HH:mm:ss.fff</para>
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>格式化后的日期时间字符串</returns>
    public static string ToDateTimeMsString(this DateTime dateTime) {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    /// <summary>
    /// 格式化为 ISO 8601 字符串
    /// <para>格式：yyyy-MM-ddTHH:mm:ss.fffZ</para>
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>格式化后的 ISO 8601 字符串</returns>
    public static string ToIso8601String(this DateTime dateTime) {
        return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    /// <summary>
    /// 格式化为中文日期字符串
    /// <para>格式：yyyy年MM月dd日</para>
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>格式化后的中文日期字符串</returns>
    public static string ToChineseDateString(this DateTime dateTime) {
        return dateTime.ToString("yyyy年MM月dd日");
    }

    /// <summary>
    /// 格式化为中文日期时间字符串
    /// <para>格式：yyyy年MM月dd日 HH时mm分ss秒</para>
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>格式化后的中文日期时间字符串</returns>
    public static string ToChineseDateTimeString(this DateTime dateTime) {
        return dateTime.ToString("yyyy年MM月dd日 HH时mm分ss秒");
    }

    /// <summary>
    /// 获取一天的开始时间
    /// <para>返回当天的 00:00:00</para>
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>当天的开始时间</returns>
    /// <example>
    /// <code>
    /// var date = new DateTime(2026, 4, 6, 14, 30, 25);
    /// var start = date.StartOfDay(); // 返回：2026-04-06 00:00:00
    /// </code>
    /// </example>
    public static DateTime StartOfDay(this DateTime dateTime) {
        return dateTime.Date;
    }

    /// <summary>
    /// 获取一天的结束时间
    /// <para>返回当天的 23:59:59.9999999</para>
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>当天的结束时间</returns>
    /// <example>
    /// <code>
    /// var date = new DateTime(2026, 4, 6, 14, 30, 25);
    /// var end = date.EndOfDay(); // 返回：2026-04-06 23:59:59.9999999
    /// </code>
    /// </example>
    public static DateTime EndOfDay(this DateTime dateTime) {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// 获取一周的开始时间（周一）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>本周一的开始时间</returns>
    public static DateTime StartOfWeek(this DateTime dateTime) {
        var diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dateTime.AddDays(-diff).Date;
    }

    /// <summary>
    /// 获取一周的结束时间（周日）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>本周日的结束时间</returns>
    public static DateTime EndOfWeek(this DateTime dateTime) {
        return dateTime.StartOfWeek().AddDays(6).EndOfDay();
    }

    /// <summary>
    /// 获取一月的开始时间
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>当月第一天的开始时间</returns>
    public static DateTime StartOfMonth(this DateTime dateTime) {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    /// <summary>
    /// 获取一月的结束时间
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>当月最后一天的结束时间</returns>
    public static DateTime EndOfMonth(this DateTime dateTime) {
        return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// 获取一年的开始时间
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>当年第一天的开始时间</returns>
    public static DateTime StartOfYear(this DateTime dateTime) {
        return new DateTime(dateTime.Year, 1, 1);
    }

    /// <summary>
    /// 获取一年的结束时间
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>当年最后一天的结束时间</returns>
    public static DateTime EndOfYear(this DateTime dateTime) {
        return new DateTime(dateTime.Year, 12, 31).EndOfDay();
    }

    /// <summary>
    /// 判断是否是今天
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>如果是今天返回 true</returns>
    public static bool IsToday(this DateTime dateTime) {
        return dateTime.Date == DateTime.Today;
    }

    /// <summary>
    /// 判断是否是工作日（周一到周五）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>如果是工作日返回 true</returns>
    public static bool IsWeekday(this DateTime dateTime) {
        return dateTime.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);
    }

    /// <summary>
    /// 判断是否是周末（周六或周日）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>如果是周末返回 true</returns>
    public static bool IsWeekend(this DateTime dateTime) {
        return !dateTime.IsWeekday();
    }

    /// <summary>
    /// 计算年龄
    /// </summary>
    /// <param name="birthDate">出生日期</param>
    /// <param name="referenceDate">参考日期，默认为今天</param>
    /// <returns>年龄</returns>
    public static int CalculateAge(this DateTime birthDate, DateTime? referenceDate = null) {
        var reference = referenceDate ?? DateTime.Today;
        var age = reference.Year - birthDate.Year;

        if (reference < birthDate.AddYears(age)) {
            age--;
        }

        return age;
    }

    /// <summary>
    /// 获取两个日期之间的天数差（绝对值）
    /// </summary>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <returns>天数差</returns>
    public static int DaysBetween(this DateTime startDate, DateTime endDate) {
        return Math.Abs((endDate.Date - startDate.Date).Days);
    }

    /// <summary>
    /// 获取两个日期之间的月数差（绝对值）
    /// </summary>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <returns>月数差</returns>
    public static int MonthsBetween(this DateTime startDate, DateTime endDate) {
        var months = Math.Abs((endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month);

        if (endDate.Day < startDate.Day) {
            months--;
        }

        return months;
    }

    /// <summary>
    /// 获取友好时间描述
    /// <para>如：刚刚、5分钟前、昨天、3天前等</para>
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <param name="referenceDate">参考时间，默认为当前时间</param>
    /// <returns>友好时间描述</returns>
    public static string ToFriendlyString(this DateTime dateTime, DateTime? referenceDate = null) {
        var reference = referenceDate ?? DateTime.Now;
        var span = reference - dateTime;

        if (span.TotalSeconds < 60) {
            return "刚刚";
        }

        if (span.TotalMinutes < 60) {
            return $"{(int)span.TotalMinutes}分钟前";
        }

        if (span.TotalHours < 24) {
            return $"{(int)span.TotalHours}小时前";
        }

        if (span.TotalDays < 2) {
            return "昨天";
        }

        if (span.TotalDays < 7) {
            return $"{(int)span.TotalDays}天前";
        }

        if (span.TotalDays < 30) {
            return $"{(int)(span.TotalDays / 7)}周前";
        }

        return span.TotalDays < 365 ? $"{(int)(span.TotalDays / 30)}个月前" : $"{(int)(span.TotalDays / 365)}年前";
    }

    /// <summary>
    /// 转换为时间戳（秒）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>Unix 时间戳（秒）</returns>
    public static long ToTimestamp(this DateTime dateTime) {
        return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 转换为时间戳（毫秒）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>Unix 时间戳（毫秒）</returns>
    public static long ToTimestampMs(this DateTime dateTime) {
        return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 从时间戳创建日期时间
    /// </summary>
    /// <param name="timestamp">Unix 时间戳（秒）</param>
    /// <returns>日期时间</returns>
    public static DateTime FromTimestamp(long timestamp) {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
    }

    /// <summary>
    /// 从毫秒时间戳创建日期时间
    /// </summary>
    /// <param name="timestampMs">Unix 时间戳（毫秒）</param>
    /// <returns>日期时间</returns>
    public static DateTime FromTimestampMs(long timestampMs) {
        return DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).LocalDateTime;
    }
}
