using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AhesselbomGenerator.Xml;

namespace AhesselbomGenerator;

public class YouTubeListGenerator
{
    private readonly string _filename;

    public YouTubeListGenerator(string filename)
    {
        _filename = Path.Combine(Config.SourceDirectory, filename);
    }

    public YouTubeListGeneratorResult Generate()
    {
        var html = new StringBuilder();
        var rss = new StringBuilder();

        var rows = Regex.Split(FileReader.GetTextFileContent(_filename), @"\n");
        html.AppendLine(@"<table border=""0"" cellspacing=""0"" cellpadding=""0"">");
        var eachOther = false;
        var y = DateTime.Now.Year.ToString("0000");

        rss.AppendLine($@"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<rss xmlns:content=""http://purl.org/rss/1.0/modules/content/"" xmlns:wfw=""http://wellformedweb.org/CommentAPI/"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:atom=""http://www.w3.org/2005/Atom"" xmlns:sy=""http://purl.org/rss/1.0/modules/syndication/"" xmlns:slash=""http://purl.org/rss/1.0/modules/slash/"" version=""2.0"">
<channel>
 <title>Veckans Hesselbom</title>
 <description>De senaste YouTube-uppladdningarna av Anders Hesselbom</description>
 <link>https://ahesselbom.se/youtube/rss.xml</link>
 <copyright>{y} Anders Hesselbom</copyright>
 <lastBuildDate>{RssHelp.FormatDate(DateTime.Now)}</lastBuildDate>
 <pubDate>{RssHelp.FormatDate(DateTime.Now)}</pubDate>
 <ttl>120</ttl>");

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row) || row.IndexOf('|') < 0)
                continue;

            var parts = row.Split('|');

            if (parts.Length < 3)
                continue;

            eachOther = !eachOther;

            html.Append("<tr>");
            html.Append($@"<td class=""ytdate {(eachOther ? "r1" : "r2")}"" style=""white-space: nowrap;"">{parts[2]}</td>");
            html.Append($@"<td class=""ytlink {(eachOther ? "r1" : "r2")}""><a href=""https://www.youtube.com/watch?v={parts[0]}"" target=""_blank"">{parts[1]}</a></td>");
            html.Append("</tr>");

            var pubDateParts = parts[2].Split('-');
            var year = int.Parse(pubDateParts[0]);
            var m = int.Parse(pubDateParts[1]);
            var day = int.Parse(pubDateParts[2]);
            var dt = new DateTime(year, m, day);

            rss.AppendLine($@" <item>
  <title>{parts[1]}</title>
  <description>Se videon på YouTube!</description>
  <link>https://www.youtube.com/watch?v={parts[0]}</link>
  <guid isPermaLink=""true"">https://www.youtube.com/watch?v={parts[0]}</guid>
  <pubDate>{RssHelp.FormatDate(dt)}</pubDate>
 </item>");
        }

        html.Append("</table>");

        rss.AppendLine(@"</channel>
</rss>");

        return new YouTubeListGeneratorResult(html.ToString(), rss.ToString());
    }
}