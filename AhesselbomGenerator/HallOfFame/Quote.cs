namespace AhesselbomGenerator.HallOfFame;

internal class Quote
{
    public string QuoteText { get; }
    public string Url { get; }

    public Quote(string quoteText, string url)
    {
        QuoteText = quoteText;
        Url = url;
    }
}