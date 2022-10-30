using System.Text;

namespace PersonalWebsiteBuilder.Widgets;

public abstract class StaticHtmlWidget : WidgetBase
{
    public string StaticHtml { get; }

    protected StaticHtmlWidget(string position, string classes, string staticHtml) : base(position, classes)
    {
        StaticHtml = staticHtml;
    }

    protected override void RenderWidgetChildPart(StringBuilder s)
    {
        s.Append(StaticHtml);
    }
}