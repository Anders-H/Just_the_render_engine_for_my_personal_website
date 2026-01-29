using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CommentCollector;

var inputFiles = new List<string>
{
    "C:/Users/hbom/OneDrive/ahesselbom.se2/Output/rss/rss_comments.xml",
    "C:/Users/hbom/OneDrive/ahesselbom.se2/Output/rss/winsoft-comments.xml"
};

const string outputFile = @"C:\Users\hbom\OneDrive\ahesselbom.se2\Source\comments.txt";

var options = new FileStreamOptions
{
    Access = FileAccess.Write,
    Mode = FileMode.Create
};

var commentList = new List<Comment>();

foreach (var inputFile in inputFiles)
    commentList.AddRange(GetComments(inputFile));

var sortedComments = commentList.OrderByDescending(x => x.PublishedTime).ToList();

using var sw = new StreamWriter(outputFile, Encoding.UTF8, options);
sw.WriteLine("<h2>Kommentarer</h2>");
var count = 0;

foreach (var comment in sortedComments)
{
    count++;

    sw.WriteLine(comment.ToHtml());

    if (count >= 5)
        break;
}

return;

IEnumerable<Comment> GetComments(string filePath)
{
    CheckFileIsXml(filePath);
    using var sr = new StreamReader(filePath);
    var xml = sr.ReadToEnd();
    sr.Close();
    var xmlStart = xml.IndexOf("<?xml", StringComparison.Ordinal);

    if (xmlStart > 0)
        xml = xml[xmlStart..].Trim();

    var dom = new XmlDocument();
    dom.LoadXml(xml);
    var namespaceManager = new XmlNamespaceManager(dom.NameTable);
    namespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
    namespaceManager.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");
    var items = dom.DocumentElement!.SelectSingleNode("channel")!.SelectNodes("item");
    var comments = new List<Comment>();
    var index = 0;

    foreach (XmlElement item in items!)
    {
        var title = item.SelectSingleNode("title")!.InnerText.Trim();
        var link = item.SelectSingleNode("link")!.InnerText.Trim();
        var publishedTime = ToDateTime(item.SelectSingleNode("pubDate")!.InnerText.Trim());
        var creator = item.SelectNodes("//dc:creator", namespaceManager)![index]!.InnerText.Trim();
        var content = item.SelectNodes("//content:encoded", namespaceManager)![index]!.InnerText.Trim();
        comments.Add(new Comment(title, link, publishedTime ?? new DateTime(1980, 1, 1, 10, 0, 0), creator, content));
        index++;
    }
    
    return comments;
}

static void CheckFileIsXml(string filename)
{
    var content = "";

    using (var sr = new StreamReader(filename))
    {
        content = sr.ReadToEnd().Trim();
        sr.Close();
    }

    if (content.StartsWith("<?xml"))
        return;

    var xmlStart = content.IndexOf("<?xml", StringComparison.Ordinal);

    if (xmlStart == -1)
        throw new Exception($"File {filename} is not valid XML.");

    content = content[xmlStart..].Trim();

    using (var sw = new StreamWriter(filename, false, Encoding.UTF8))
    {
        sw.Write(content);
        sw.Flush();
        sw.Close();
    }
}

DateTime? ToDateTime(string? feedDate)
{
    if (string.IsNullOrWhiteSpace(feedDate))
        return null;

    var hit = Regex.Match(feedDate, @"([0-9]+)\s([A-Z][a-z]+)\s(20[0-9][0-9])\s([0-9]+):([0-9]+):([0-9]+)", RegexOptions.IgnorePatternWhitespace);

    if (!hit.Success)
        return null;

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
        var hour = int.Parse(hit.Groups[4].Value);
        var minute = int.Parse(hit.Groups[5].Value);
        var second = int.Parse(hit.Groups[6].Value);

        return new DateTime(year, month, date, hour, minute, second).AddHours(1);
    }
    catch
    {
        return null;
    }
}