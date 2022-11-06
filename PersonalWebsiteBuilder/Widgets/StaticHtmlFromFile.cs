using System.Text;

namespace PersonalWebsiteBuilder.Widgets;

public class StaticHtmlFromFile : StaticHtmlWidget
{
    public StaticHtmlFromFile(WebSite site, string filename) : base("Right", site, "card")
    {
        StaticHtml = GetFromFile(filename);
    }

    public override bool ShouldRenderOn(WebPage page) =>
        false;

    private string GetFromFile(string filename)
    {
        var path = Path.Combine(Site.StaticResourcesDirectory.FullName, filename);
        return File.ReadAllText(path, Encoding.UTF8);
    }
}