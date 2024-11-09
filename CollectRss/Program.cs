using System.Globalization;
using System.Text;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

var rssFiles = new List<FeedFile>
{
    new("X (Twitter)", @"C:\Users\hbom\OneDrive\ahesselbom.se2\Output\rss\ahesselbom_x_rss.xml", true, true),
    new("Bloggen", @"C:\Users\hbom\OneDrive\ahesselbom.se2\Output\rss\rss.xml", false, false),
    new("YouTube (Veckans Hesselbom)", @"C:\Users\hbom\OneDrive\ahesselbom.se2\Output\rss\veckanshesselbom_rss.xml", true, false),
    new("Teknikbloggen", @"C:\Users\hbom\OneDrive\ahesselbom.se2\Output\rss\winsoft.xml", false, false),
    new("Podcast (Inte en singel)", @"C:\Users\hbom\OneDrive\InteEnSingel\Output\rss.xml", true, true),
    new("Podcast (Blev det en klassiker?)", @"C:\Users\hbom\OneDrive\BlevDetEnKlassiker\Output\rss.xml", true, true),
    new("YouTube (Flimmer Duo)", @"C:\Users\hbom\OneDrive\80tal\Output\feed\tag\flimmer-duo.xml", true, false)
};

const string output = @"C:\Users\hbom\OneDrive\ahesselbom.se2\Source\start_content.txt";
var items = new List<Item>();

foreach (var f in rssFiles)
{
    var xmlList = f.GetItems(10);

    foreach (var xmlElement in xmlList)
    {
        var dateString = xmlElement.SelectSingleNode("pubDate");

        if (dateString == null)
            throw new SystemException($"pubDate in {f.Name}");

        const string parseFormat = "ddd, dd MMM yyyy HH:mm:ss zzz";
        var dateStringValue = dateString.InnerText;
        dateStringValue = dateStringValue.Replace(" GMT", " +0000");
        var date = DateTime.ParseExact(dateStringValue, parseFormat, CultureInfo.InvariantCulture);
        var header = xmlElement.SelectSingleNode("title")?.InnerText ?? "";
        var text = xmlElement.SelectSingleNode("description")?.InnerText ?? "";
        var url = xmlElement.SelectSingleNode("link")?.InnerText ?? "";
        Console.WriteLine($"Adding from {f.Name} created at {date.ToShortDateString()} {date.ToShortTimeString()}.");
        var item = new Item(f.Name, f.NewTab, f.IncludeTime, date, header, text, url);
        items.Add(item);
    }
}

var sortedItems = new List<Item>();
sortedItems.AddRange(items.OrderByDescending(x => x.Date));
using var streamWriter = new StreamWriter(output, Encoding.UTF8, new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.Create});

foreach (var item in sortedItems)
{
    Console.WriteLine($"Writing from {item.FeedName} created at {item.Date.ToShortDateString()} {item.Date.ToShortTimeString()}.");
    streamWriter.WriteLine(item.GetHtml());
}

streamWriter.Flush();
streamWriter.Close();

public class FeedFile
{
    public string Name { get; }
    public string Filename { get; }
    public bool NewTab { get; }
    public bool IncludeTime { get;}

    public FeedFile(string name, string filename, bool newTab, bool includeTime)
    {
        Name = name;
        Filename = filename;
        NewTab = newTab;
        IncludeTime = includeTime;
    }

    public List<XmlElement> GetItems(int maxCount)
    {
        var dom = new XmlDocument();
        dom.Load(Filename);

        if (dom.DocumentElement == null)
            throw new SystemException($"Document element is null: {Name}");

        var channel = dom.DocumentElement.SelectSingleNode("channel");

        if (channel == null)
            throw new SystemException($"Channel element is null: {Name}");

        var result = new List<XmlElement>();

        foreach (XmlElement item in channel.ChildNodes)
        {
            if (item.Name != "item")
                continue;

            result.Add(item);
            
            if (result.Count >= maxCount)
                break;
        }

        return result;
    }
}

public class Item
{
    public string FeedName { get; }
    public bool NewTab { get; }
    public bool IncludeTime { get; }
    public DateTime Date { get; }
    public string Header { get; }
    public string Text { get; }
    public string Url { get; }

    public Item(string feedName, bool newTab, bool includeTime, DateTime date, string header, string text, string url)
    {
        FeedName = feedName;
        NewTab = newTab;
        IncludeTime = includeTime;
        Date = date;
        Header = header;
        Text = text;
        Url = url;
    }

    public string OpenAnchor =>
        NewTab ? $@"<a href=""{Url}"" target=""_blank"">" : $@"<a href=""{Url}"">";

    public string DateString =>
        IncludeTime ? Date.ToString("yyyy-MM-dd hh:mm") : Date.ToString("yyyy-MM-dd");

    private string TwitterHeader =>
        Header.Length > 50 ? Header.Substring(0, 45).Trim() + "..." : Header;

    public string GetHtml()
    {
        var s = new StringBuilder();
        var text = Text.Replace("[&#8230;]", $@"<a href=""{Url}"">[&#8230;]</a>");

        switch (FeedName)
        {
            case "X (Twitter)":
                s.Append("<p>");
                s.Append(OpenAnchor);
                s.Append($@"<i style=""font-weight: lighter;"">{FeedName}:</i> <b>{TwitterHeader}</b>");
                s.Append("</a>");
                s.Append($"<br/><i>{DateString}</i>");
                s.Append("</p>");
                s.Append("<p>");
                s.Append(Header);
                s.Append(" ");
                s.Append(OpenAnchor);
                s.Append("Visa inlägg");
                s.Append("</a>");
                s.Append("</p>");
                break;
            case "YouTube (Veckans Hesselbom)":
            case "YouTube (Flimmer Duo)":
                s.Append("<p>");
                s.Append(OpenAnchor);
                s.Append($@"<i style=""font-weight: lighter;"">{FeedName}:</i> <b>{Header}</b>");
                s.Append("</a>");
                s.Append($"<br/><i>{DateString}</i>");
                s.Append("</p>");
                s.Append("<p>");
                s.Append(text);
                s.Append(" ");
                s.Append(OpenAnchor);
                s.Append(Url);
                s.Append("</a>");
                s.Append("</p>");
                break;
            default:
                s.Append("<p>");
                s.Append(OpenAnchor);
                s.Append($@"<i style=""font-weight: lighter;"">{FeedName}:</i> <b>{Header}</b>");
                s.Append("</a>");
                s.Append($"<br/><i>{DateString}</i>");
                s.Append("</p>");
                s.Append("<p>");
                s.Append(text);
                s.Append("</p>");
                break;
        }

        return s.ToString();
    }
}