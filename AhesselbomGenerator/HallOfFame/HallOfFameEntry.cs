using System.Collections.Generic;

namespace AhesselbomGenerator.HallOfFame;

internal class HallOfFameEntry
{
    public string Name { get; }
    public string Cv { get; }
    public string Image { get; }
    public string ImageCredit { get; }
    public string ImageCreditUrl { get; }
    public string Page { get; }
    public List<Quote> Quotes { get; }

    public HallOfFameEntry(string name, string cv, string image, string imageCredit, string imageCreditUrl, string page)
    {
        Name = name;
        Cv = cv;
        Image = image;
        ImageCredit = imageCredit;
        ImageCreditUrl = imageCreditUrl;
        Page = page;
        Quotes = [];
    }
}