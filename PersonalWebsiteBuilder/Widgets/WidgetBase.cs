using System.Text;

namespace PersonalWebsiteBuilder.Widgets;

public abstract class WidgetBase
{
    protected readonly string Classes;
    protected readonly WebSite Site;
    public readonly string Position;

    protected WidgetBase(string position, WebSite site, string classes)
    {
        Classes = classes;
        Site = site;
        Position = position;
    }

    public abstract bool ShouldRenderOn(WebPage page);

    public string Render(WebPage page)
    {
        var s = new StringBuilder();
        s.Append($@"<div class=""{Classes}"">");
        RenderWidgetChildPart(s, page);
        s.Append("</div>");
        return s.ToString();
    }

    protected abstract void RenderWidgetChildPart(StringBuilder s, WebPage page);
}