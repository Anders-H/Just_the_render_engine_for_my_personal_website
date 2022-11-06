using System.Net.Http.Headers;
using System.Text;

namespace PersonalWebsiteBuilder.Widgets;

public class MenuWidget : WidgetBase
{
    public MenuWidget(string position, WebSite site, string classes) : base(position, site, classes)
    {
    }

    public override bool ShouldRenderOn(WebPage page) =>
        true;

    protected override void RenderWidgetChildPart(StringBuilder s, WebPage page)
    {
        if (Position == "Right")
            s.Append("<h3>Innehåll</h3>");

        s.Append(@"<div id=""menu"">");
        s.Append(@"<a href=""https://ahesselbom.se/"" class=""menulink selected"">Startsidan</a>");
        s.Append(@"</div>");
    }

    private string GetLink(string url, WebPage page, string name, string currentName)
    {
        var selected = page.PageName == "name";
        var classes = selected ? "menulink selected" : "menulink";
        // TODO
    }
}