using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AhesselbomGenerator;

public class YouTubeListGenerator
{
    private readonly string _filename;

    public YouTubeListGenerator(string filename)
    {
        _filename = Path.Combine(Config.SourceDirectory, filename);
    }

    public string Generate()
    {
        var s = new StringBuilder();
        var rows = Regex.Split(FileReader.GetTextFileContent(_filename), @"\n");
        s.AppendLine(@"<table border=""0"" cellspacing=""0"" cellpadding=""0"">");
        var eachother = false;

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row) || row.IndexOf('|') < 0)
                continue;

            var parts = row.Split('|');

            if (parts.Length < 3)
                continue;

            eachother = !eachother;

            s.Append("<tr>");
            s.Append($@"<td class=""ytdate {(eachother ? "r1" : "r2")}"" style=""white-space: nowrap;"">{parts[2]}</td>");
            s.Append($@"<td class=""ytlink {(eachother ? "r1" : "r2")}""><a href=""https://www.youtube.com/watch?v={parts[0]}"" target=""_blank"">{parts[1]}</a></td>");
            s.Append("</tr>");
        }
        s.Append("</table>");
        return s.ToString();
    }
}