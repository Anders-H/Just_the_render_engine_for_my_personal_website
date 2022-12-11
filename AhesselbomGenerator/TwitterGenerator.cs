using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AhesselbomGenerator;

public class TwitterGenerator
{
    private const string Source = "http://fetchrss.com/rss/5fa97ace54a1393bc21bf3525fa979ff58e79d2e7e2f67b2.xml";
    private readonly string _temp;

    public TwitterGenerator()
    {
        _temp = Path.Combine(Path.GetTempPath(), "temp.xml");
    }

    public string Generate()
    {
        var dom = new XmlDocument();
        FileReader.DownloadTextFile(Source, _temp);
        dom.Load(_temp);
        var rss = dom.DocumentElement;

        if (rss == null)
            throw new Exception();

        if (rss.SelectSingleNode("channel") is not XmlElement channel)
            throw new Exception();

        var items = channel.SelectNodes("item");

        if (items == null)
            throw new Exception();

        var s = new StringBuilder();

        foreach (XmlElement item in items)
        {
            var link = item.SelectSingleNode("link")?.InnerText ?? "";
            var t = item.SelectSingleNode("title")?.InnerText ?? "";

            s.Append("<p>");
            s.Append("</p>");

            s.Append("<p>");
            s.AppendLine($@"<a href=""{link}"" target=""_blank"">{t}</a>");
            s.Append("</p>");
        }

        return s.ToString();
    }
}