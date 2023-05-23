using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using AhesselbomGenerator.Xml;

namespace AhesselbomGenerator;

public class BloggGenerator
{
    private readonly string _filename;
    private readonly string _temp;
    private readonly int _max;

    public BloggGenerator(string filename)
    {
        _filename = filename.StartsWith("http")
            ? filename
            : Path.Combine(Config.SourceDirectory, filename);

        _temp = Path.Combine(Path.GetTempPath(), "temp.xml");

        _max = int.MaxValue;
    }

    public BloggGenerator(string filename, int max)
    {
        _filename = filename.StartsWith("http")
            ? filename
            : Path.Combine(Config.SourceDirectory, filename);

        _temp = Path.Combine(Path.GetTempPath(), "temp.xml");

        _max = max;
    }

    public string Generate(bool full) =>
        full
            ? GenerateFull()
            : GenerateHeadersList();

    public string Generate(bool full, int skip) =>
        full
            ? GenerateFull(skip)
            : GenerateHeadersList(skip);

    private string GenerateFull(int skip = 0)
    {
        var dom = new XmlDocument();

        if (_filename.StartsWith("http"))
        {
            FileReader.DownloadTextFile(_filename, _temp);
            dom.Load(_temp);
        }
        else
        {
            dom.Load(_filename);
        }

        var items = dom.GetItemsOrThrow();

        var count = 0;
        
        var s = new StringBuilder();
        
        foreach (XmlElement item in items)
        {
            if (skip > 0 && count < skip)
            {
                count++;
                continue;
            }

            if (count >= _max + skip)
                break;

            count++;

            var link = item.SelectSingleNode("link")?.InnerText ?? "";
            var header = item.SelectSingleNode("title")?.InnerText ?? "";
            var dateString = ToDateString(item.SelectSingleNode("pubDate")?.InnerText);
            dateString = string.IsNullOrEmpty(dateString) ? "" : $@"<br /><i>{dateString}</i>";

            s.AppendLine($@"<p><b><a href=""{link}"">{header}</a></b>{dateString}</p>");
            var text = item.SelectSingleNode("description")?.InnerText ?? "";
            text = text.Replace("<br />", "<br /><br />");
            text = text.Replace("<br /><br /><br /><br />", "<br /><br />");
            text = text.Replace("<br /><br /><br /><br />", "<br /><br />");
            text = text.Replace("<br /><br /><br />", "<br /><br />");
            text = text.Replace("<br /><br /><blockquote", "<br /><blockquote");
            text = text.Replace("</i><br /><br /><i><br /><br /></i>", "</i><br /><br />");
            s.AppendLine($"<p>{text}</p>");
        }

        return s.ToString().Replace("<br /><br />", "<br />");
    }

    private string GenerateHeadersList(int skip = 0)
    {
        var dom = new XmlDocument();

        if (_filename.StartsWith("http"))
        {
            FileReader.DownloadTextFile(_filename, _temp);
            dom.Load(_temp);
        }
        else
        {
            dom.Load(_filename);
        }

        var rss = dom.DocumentElement;
        
        if (rss == null)
            throw new Exception();

        if (rss.SelectSingleNode("channel") is not XmlElement channel)
            throw new Exception();
        
        var items = channel.SelectNodes("item");
        
        if (items == null)
            throw new Exception();
        
        var count = 0;
        
        var s = new StringBuilder();
        
        var added = false;
        
        foreach (XmlElement item in items)
        {
            if (skip > 0 && count < skip)
            {
                count++;
                continue;
            }

            if (count >= _max + skip)
                break;

            count++;

            if (added)
                s.Append("<br />");

            var dateString = ToDateString(item.SelectSingleNode("pubDate")?.InnerText);
            dateString = string.IsNullOrEmpty(dateString) ? "" : $@" ({dateString})";

            s.AppendLine($@"<a href=""{item.SelectSingleNode("link")?.InnerText ?? ""}"">{item.SelectSingleNode("title")?.InnerText ?? ""}</a>{dateString}");
            added = true;
        }

        return s.ToString();
    }

    public static string ToDateString(string? feedDate)
    {
        if (string.IsNullOrWhiteSpace(feedDate))
            return "";

        var hit = Regex.Match(feedDate, @"([0-9]+)\s([A-Z][a-z]+)\s(20[0-9][0-9])", RegexOptions.IgnorePatternWhitespace);
        
        if (!hit.Success)
            return "";

        try
        {
            var date = int.Parse(hit.Groups[1].Value);
            var month = hit.Groups[2].Value switch
            {
                "Jan" => 1,
                "Feb" => 2,
                "Mar" => 3,
                "Apr" => 4,
                "May" => 5,
                "Jun" => 6,
                "Jul" => 7,
                "Aug" => 8,
                "Sep" => 9,
                "Oct" => 10,
                "Nov" => 11,
                "Dec" => 12,
                _ => throw new SystemException("Nothing works here...")
            };
            var year = int.Parse(hit.Groups[3].Value);
            return new DateTime(year, month, date).ToShortDateString();
        }
        catch
        {
            return "";
        }
    }
}