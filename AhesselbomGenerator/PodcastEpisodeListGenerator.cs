using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AhesselbomGenerator;

public class PodcastEpisodeListGenerator
{
    private readonly string _filename;
    private readonly string _temp;

    public PodcastEpisodeListGenerator(string filename)
    {
        _filename = filename.StartsWith("http")
            ? filename
            : Path.Combine(Config.SourceDirectory, filename);

        _temp = Path.Combine(Path.GetTempPath(), "temp.xml");
    }

    public string Generate()
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

        var s = new StringBuilder();

        var avsnitt = items.Count;

        foreach (XmlElement item in items)
        {
            s.Append("<p>");
            s.Append($"<b>Avsnitt {avsnitt}: {item.SelectSingleNode("title").InnerText}</b>");
            s.Append("<br />");
            var mp3 = item!.SelectSingleNode("enclosure")!.Attributes!.GetNamedItem("url")!.Value;
            s.Append($@"<a href=""{mp3}"" target=""_blank"">Lyssna ({mp3})</a>");
            s.Append("</p>");

            avsnitt--;
        }

        return s.ToString();
    }
}