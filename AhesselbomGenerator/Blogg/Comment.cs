using System.Linq;
using System.Web;
using System.Xml;

namespace AhesselbomGenerator.Blogg;

public class Comment
{
    public string PageUrl { get; }
    public string Date { get; }
    public string By { get; }

    public Comment(XmlElement element)
    {
        PageUrl = element.SelectSingleNode("link")?.InnerText ?? "";
        Date = BloggGenerator.ToDateString(element.SelectSingleNode("pubDate")?.InnerText ?? "");
        By = GetCommenter(element);
    }

    private string GetCommenter(XmlElement element)
    {
        foreach (var e in element.Cast<XmlElement>().Where(e => e.Name == "dc:creator"))
            return e.InnerText;

        return "";
    }

    public string GetHtml() =>
        $@"<a href=""{PageUrl}"" style=""font-weight: normal; color: #777777; font-size: smaller;"">{HttpUtility.HtmlEncode(By)} ({Date})</a>";
}