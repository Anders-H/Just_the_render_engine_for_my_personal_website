namespace AhesselbomGenerator;

public class YouTubeListGeneratorResult
{
    public string Html { get; }
    public string Rss { get; }

    public YouTubeListGeneratorResult(string html, string rss)
    {
        Html = html;
        Rss = rss;
    }
}