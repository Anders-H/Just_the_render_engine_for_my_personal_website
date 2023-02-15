using System;
using System.IO;
using System.Text;
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

        var rss = dom.DocumentElement;
        
        if (rss == null)
            throw new Exception();

        if (rss.SelectNode("channel") is not XmlElement channel)
            throw new Exception();
        
        var items = channel.SelectNodes("item");
        
        if (items == null)
            throw new Exception();
        
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

            s.AppendLine($@"<p><b><a href=""{link}"">{header}</a></b></p>");
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

            s.AppendLine($@"<a href=""{item.SelectSingleNode("link")?.InnerText ?? ""}"">{item.SelectSingleNode("title")?.InnerText ?? ""}</a>");
            added = true;
        }

        return s.ToString();
    }
}