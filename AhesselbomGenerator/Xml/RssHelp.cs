using System;

namespace AhesselbomGenerator.Xml;

public class RssHelp
{
    public static string FormatDate(DateTime dt)
    {
        var y = DateTime.Now.Year.ToString("0000");
        var h = DateTime.Now.Hour.ToString("00");
        var m = DateTime.Now.Minute.ToString("00");
        var d = DateTime.Now.Day.ToString("00");
        var dayOfWeek = DayOfWeekAsString(DateTime.Now.DayOfWeek);
        var month = MonthAsString(DateTime.Now.Month);
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