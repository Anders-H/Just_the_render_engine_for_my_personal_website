using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AhesselbomGenerator;

public class BloggGenerator
{
    private readonly string _filename;
    private readonly string _temp;

    public BloggGenerator(string filename)
    {
        _filename = filename.StartsWith("http")
            ? filename
            : Path.Combine(Config.SourceDirectory, filename);

        _temp = Path.Combine(Path.GetTempPath(), "temp.xml");
    }

    public string Generate(bool full) =>
        full
            ? GenerateFull()
            : GenerateHeadersList();

    private string GenerateFull()
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
        
        var channel = (XmlElement)rss.SelectSingleNode("channel");
        
        if (channel == null)
            throw new Exception();
        
        var items = channel.SelectNodes("item");
        
        if (items == null)
            throw new Exception();
        
        var count = 0;
        
        var s = new StringBuilder();
        
        foreach (XmlElement item in items)
        {
            if (count >= 50)
                break;

            var link = item.SelectSingleNode("link")?.InnerText ?? "";
            var header = (item.SelectSingleNode("title")?.InnerText ?? "").ToUpper();

            s.AppendLine($@"<p><b><a href=""{link}"" target=""_blank"">*** {header} ***</a></b></p>");
            var text = item.SelectSingleNode("description")?.InnerText ?? "";
            text = text.Replace("<br />", "<br /><br />");
            text = text.Replace("<br /><br /><br /><br />", "<br /><br />");
            text = text.Replace("<br /><br /><br /><br />", "<br /><br />");
            text = text.Replace("<br /><br /><br />", "<br /><br />");
            text = text.Replace("<br /><br /><blockquote", "<br /><blockquote");
            text = text.Replace("</i><br /><br /><i><br /><br /></i>", "</i><br /><br />");
            s.AppendLine($"<p>{text}</p>");
            count++;
        }

        return s.ToString().Replace("<br /><br />", "<br />");
    }

    private string GenerateHeadersList()
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
        
        var channel = (XmlElement)rss.SelectSingleNode("channel");
        
        if (channel == null)
            throw new Exception();
        
        var items = channel.SelectNodes("item");
        
        if (items == null)
            throw new Exception();
        
        var count = 0;
        
        var s = new StringBuilder();
        
        var added = false;
        
        foreach (XmlElement item in items)
        {
            if (count >= 100)
                break;

            if (added)
                s.Append("<br />");

            s.AppendLine($@"<a href=""{item.SelectSingleNode("link")?.InnerText ?? ""}"" target=""_blank"">{item.SelectSingleNode("title")?.InnerText ?? ""}</a>");
            added = true;
            count++;
        }

        return s.ToString();
    }
}