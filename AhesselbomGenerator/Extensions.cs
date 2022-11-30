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
}