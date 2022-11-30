namespace AhesselbomGenerator.HtmlProcessors;

public class EmbedProcessor
{
    private readonly string _row;

    public EmbedProcessor(string row)
    {
        _row = row;
    }

    public string Process()
    {
        var videoId = _row.ExtractValue();
        return $@"
<div style=""width: 420px; height: 305px;"">
   <iframe src=""https://www.youtube.com/embed/{videoId}?controls=0&showinfo=0&autoplay=0&loop=0&mute=0"" frameborder=""0"" style=""width: 420px; height: 290px;"" allowfullscreen></iframe>
</div>";
    }
}