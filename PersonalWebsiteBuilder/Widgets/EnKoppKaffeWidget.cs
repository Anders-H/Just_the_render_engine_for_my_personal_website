namespace PersonalWebsiteBuilder.Widgets;

public class EnKoppKaffeWidget : StaticHtmlWidget
{
    private const string staticHtml = @"<h2>En kopp kaffe!</h2>
<div class=""sideImageContainer"" style=""height: 100%;"">
<p>Bjud mig på en kopp kaffe (20:-) som tack för bra innehåll!</p>
<p style=""display: block; position: relative; height:100%; width:100%;"">
<img src=""https://ahesselbom.se/img/swish.png"" alt=""Bjud på en kopp kaffe!"" style=""display: block; max-height: 100%; max-width: 100%; height:100%; width:100%;"" />
</p>
</div>";

    public EnKoppKaffeWidget() : base("Right", "card", staticHtml)
    {
    }

    public override bool ShouldRenderOn(WebPage page) =>
        false;
}