using System;
using System.IO;
using System.Text;
using System.Xml;
using AhesselbomGenerator.Xml;

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

        if (rss.SelectSingleNode("channel") is not XmlElement channel)
            throw new Exception();

        var items = channel.SelectNodes("item");

        if (items == null)
            throw new Exception();

        var s = new StringBuilder();

        var avsnitt = items.Count;

        foreach (XmlElement item in items)
        {
            s.Append("<p>");

            var title = item.GetText("title");

            if (title.StartsWith("Avsnitt ", StringComparison.CurrentCultureIgnoreCase))
                s.Append($"<b>{item.GetText("title")}</b><br />");
            else
                s.Append($"<b>Avsnitt {avsnitt}: {item.GetText("title")}</b><br />");

            var mp3 = item.SelectNode("enclosure")!.GetAttributeValue("url");
            
            s.Append($@"<i><a href=""{mp3}"" target=""_blank"">Lyssna ({mp3})</a></i>");
            s.Append("</p>");

            avsnitt--;
        }

        return s.ToString();
    }
}