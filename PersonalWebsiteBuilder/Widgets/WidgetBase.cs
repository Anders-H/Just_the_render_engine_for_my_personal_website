using System.Text;

namespace PersonalWebsiteBuilder.Widgets;

public abstract class WidgetBase
{
    protected readonly string Classes;
    public readonly string Position;

    protected WidgetBase(string position, string classes)
    {
        Classes = classes;
        Position = position;
    }

    public abstract bool ShouldRenderOn(WebPage page);

    public string Render()
    {
        var s = new StringBuilder();
        s.Append($@"<div class=""{Classes}"">");
        RenderWidgetChildPart(s);
        s.Append("</div>");
        return s.ToString();
    }

    protected abstract void RenderWidgetChildPart(StringBuilder s);
}