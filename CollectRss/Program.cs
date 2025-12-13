using System.Globalization;
using System.Text;
using System.Xml;

var rssFiles = new List<FeedFile>
{
    new("X (Twitter)", @"C:\Users\hbom\OneDrive\ahesselbom.se2\Output\rss\ahesselbom_x_rss.xml", true, true),
    new("Bloggen", @"C:\Users\hbom\OneDrive\ahesselbom.se2\Output\rss\rss.xml", false, false),
    new("YouTube (Veckans Hesselbom)", @"C:\Users\hbom\OneDrive\ahesselbom.se2\Output\rss\veckanshesselbom_rss.xml", true, false),
    new("Teknikbloggen", @"C:\Users\hbom\OneDrive\ahesselbom.se2\Output\rss\winsoft.xml", false, false),
    //w("Podcast (Inte en singel)", @"C:\Users\hbom\OneDrive\InteEnSingel\Output\rss.xml", true, true),
    new("Podcast (Blev det en klassiker?)", @"C:\Users\hbom\OneDrive\BlevDetEnKlassiker\Output\rss.xml", true, true),
    new("YouTube (Flimmer Duo)", @"C:\Users\hbom\OneDrive\80tal\Output\feed\tag\flimmer-duo.xml", true, false)
};

const string output = @"C:\Users\hbom\OneDrive\ahesselbom.se2\Source\start_content.txt";
var items = new List<Item>();

foreach (var f in rssFiles)
{
    var xmlList = f.GetItems(5);

    foreach (var xmlElement in xmlList)
    {
        var dateString = xmlElement.SelectSingleNode("pubDate");

        if (dateString == null)
            throw new SystemException($"pubDate in {f.Name}");

        const string parseFormat = "ddd, dd MMM yyyy HH:mm:ss zzz";
        var dateStringValue = dateString.InnerText;
        dateStringValue = dateStringValue.Replace(" GMT", " +0000");
        Console.WriteLine(dateStringValue);
        var date = DateTime.ParseExact(dateStringValue, parseFormat, CultureInfo.InvariantCulture);

        if (f.Name.StartsWith("Podcast"))
            date = new DateTime(date.Year, date.Month, date.Day, 10, 0, 0);

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
var fileStreamOptions = new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.Create };
using var streamWriter = new StreamWriter(output, Encoding.UTF8, fileStreamOptions);
var bail = 0;

foreach (var item in sortedItems)
{
    bail++;
    var date = item.Date.ToShortDateString();
    var time = item.Date.ToShortTimeString();
    Console.WriteLine($"Writing from {item.FeedName} created at {date} {time}.");
    streamWriter.WriteLine(item.GetHtml());

    if (bail>=25)
        break;
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

    public string GetOpenAnchor(string className)
    {
        var cls = string.IsNullOrWhiteSpace(className) ? "" : $@" class=""{className}""";
        
        return NewTab
            ? $@"<a{cls} href=""{Url}"" target=""_blank"">"
            : $@"<a{cls} href=""{Url}"">";
    }

    public string DateString =>
        Date.ToString("yyyy-MM-dd");

    private string TwitterHeader =>
        Header.Length > 50 ? Header.Substring(0, 45).Trim() + "..." : Header;

    public string GetHtml()
    {
        var s = new StringBuilder();
        var addVisaInlagg = Text.IndexOf("[&#8230;]") < 0;
        var text = Text.Replace("[&#8230;]", $@"<a href=""{Url}"">[&#8230;]</a>");

        switch (FeedName)
        {
            case "X (Twitter)":
                s.Append(@"<p class=""startItemParagraph"">");
                s.Append(GetOpenAnchor("startHeaderLink"));
                s.Append($@"<img src=""./img/x.png"" alt=""X (Twitter)"" class=""startIcon""> <b class=""startHeader"">{TwitterHeader}</b>");
                s.Append("</a>");
                s.Append($"<br><i>{DateString}</i>");
                s.Append("</p>");
                s.Append("<p>");
                s.Append(Header);
                s.Append(" ");
                s.Append(GetOpenAnchor(""));
                s.Append("Visa inlägg");
                s.Append("</a>");
                s.Append("</p>");
                break;
            case "YouTube (Veckans Hesselbom)":
            case "YouTube (Flimmer Duo)":
            case "Bloggen":
            case "Teknikbloggen":
            case "Podcast (Blev det en klassiker?)":
                string icon;

                switch (FeedName)
                {
                    case "YouTube (Veckans Hesselbom)":
                        icon = "veckanshesselbom";
                        break;
                    case "YouTube (Flimmer Duo)":
                        icon = "flimmerduo";
                        break;
                    case "Bloggen":
                        icon = "bloggicon";
                        break;
                    case "Teknikbloggen":
                        icon = "winsoft";
                        break;
                    case "Podcast (Blev det en klassiker?)":
                        icon = "mic";
                        break;
                    default:
                        throw new SystemException("You suck!");
                }

                s.Append(@"<p class=""startItemParagraph"">");
                s.Append(GetOpenAnchor("startHeaderLink"));
                s.Append($@"<img src=""./img/{icon}.png"" alt=""{FeedName}"" class=""startIcon""> <b class=""startHeader"">{Header}</b>");
                s.Append("</a>");
                s.Append($"<br><i>{DateString}</i>");
                s.Append("</p>");
                s.Append("<p>");
                s.Append(text);
                
                if (addVisaInlagg)
                {
                    s.Append(" ");
                    s.Append(GetOpenAnchor(""));
                    s.Append(Url);
                    s.Append("</a>");
                }

                s.Append("</p>");
                break;
            default:
                s.Append(@"<p class=""startItemParagraph"">");
                s.Append(GetOpenAnchor("startHeaderLink"));
                s.Append($@"<i style=""font-weight: lighter;"">{FeedName}:</i> <b class=""startHeader"">{Header}</b>");
                s.Append("</a>");
                s.Append($"<br><i>{DateString}</i>");
                s.Append("</p>");
                s.Append("<p>");
                s.Append(text);
                s.Append("</p>");
                break;
        }

        return s.ToString();
    }
}