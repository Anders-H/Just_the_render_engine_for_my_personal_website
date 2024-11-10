using System;

namespace AhesselbomGenerator.Xml;

public class RssHelp
{
    public static string FormatDate(DateTime dt)
    {
        var y = dt.Year.ToString("0000");
        var h = dt.Hour.ToString("00");
        var m = dt.Minute.ToString("00");
        var d = dt.Day.ToString("00");
        var dayOfWeek = DayOfWeekAsString(dt.DayOfWeek);
        var month = MonthAsString(dt.Month);
        return $"{dayOfWeek}, {d} {month} {y} {h}:{m}:00 +0200";
    }

    private static string MonthAsString(int month) =>
        month switch
        {
            1 => "Jan",
            2 => "Feb",
            3 => "Mar",
            4 => "Apr",
            5 => "May",
            6 => "Jun",
            7 => "Jul",
            8 => "Aug",
            9 => "Sep",
            10 => "Oct",
            11 => "Nov",
            12 => "Dec",
            _ => throw new ArgumentOutOfRangeException()
        };

    private static string DayOfWeekAsString(DayOfWeek dayOfWeek) =>
        dayOfWeek switch
        {
            DayOfWeek.Sunday => "Sun",
            DayOfWeek.Monday => "Mon",
            DayOfWeek.Tuesday => "Tue",
            DayOfWeek.Wednesday => "Wed",
            DayOfWeek.Thursday => "Thu",
            DayOfWeek.Friday => "Fri",
            DayOfWeek.Saturday => "Sat",
            _ => throw new ArgumentOutOfRangeException()
        };
}