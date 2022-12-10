using System;

namespace AhesselbomGenerator;

public static class Extensions
{
    public static string ExtractValue(this string row)
    {
        var delimiter = row.IndexOf(':') + 1;
        var result = row.Substring(delimiter, row.Length - delimiter - 3).Trim();
            
        if (result.IndexOf("[PATH]", StringComparison.Ordinal) <= -1)
            return result;

        var s = Config.SourceDirectory;

        if (!s.EndsWith("/"))
            s += "/";
            
        result = result.Replace("[PATH]", s);
            
        return result;
    }

    public static (string, string) ExtractValues(this string row)
    {
        var parts = row.Split(':');

        parts[2] = parts[2].Substring(0, parts[2].Length - 3);

        if (parts[1].IndexOf("[PATH]", StringComparison.Ordinal) <= -1 && parts[2].IndexOf("[PATH]", StringComparison.Ordinal) <= -1)
            return (parts[1], parts[2]);

        var s = Config.SourceDirectory;

        if (!s.EndsWith("/"))
            s += "/";

        parts[1] = parts[1].Replace("[PATH]", s);
        parts[2] = parts[2].Replace("[PATH]", s);
        return (parts[1], parts[2]);
    }
}