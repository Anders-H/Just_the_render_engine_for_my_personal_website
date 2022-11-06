using System.Text;

namespace PersonalWebsiteBuilder.Widgets;

public abstract class StaticHtmlWidget : WidgetBase
{
    public string StaticHtml { get; protected set; }

    protected StaticHtmlWidget(string position, WebSite site, string classes) : base(position, site, classes)
    {
        StaticHtml = "";
    }

    protected StaticHtmlWidget(string position, WebSite site, string classes, string staticHtml) : base(position, site, classes)
    {
        StaticHtml = staticHtml;
    }

    protected override void RenderWidgetChildPart(StringBuilder s, WebPage page)
    {
        s.Append(StaticHtml);
    }
}