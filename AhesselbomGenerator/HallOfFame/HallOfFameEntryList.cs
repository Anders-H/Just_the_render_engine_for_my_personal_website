using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AhesselbomGenerator.HallOfFame;

internal class HallOfFameEntryList : List<HallOfFameEntry>
{
    public void Load(string filename)
    {
        var dom = new XmlDocument();

        using (var x = new StreamReader(filename, Encoding.UTF8))
        {
            dom.LoadXml(x.ReadToEnd());
            x.Close();
        }

        if (dom.DocumentElement == null)
            return;

        foreach (XmlNode xEntry in dom.DocumentElement)
        {
            var xCredit = xEntry.SelectSingleNode("imagecredit");
            var xCreditUrl = xCredit?.Attributes?.GetNamedItem("url");
            
            var entry = new HallOfFameEntry(
                xEntry.SelectSingleNode("name")?.InnerText ?? "",
                xEntry.SelectSingleNode("cv")?.InnerText ?? "",
                xEntry.SelectSingleNode("image")?.InnerText ?? "",
                xCredit?.InnerText ?? "",
                xCreditUrl?.Value,
                xEntry.SelectSingleNode("page")?.InnerText ?? ""
            );
            
            Add(entry);
            
            var quoteList = xEntry.SelectSingleNode("quotes")?.SelectNodes("quote");
            
            if (quoteList == null)
                continue;
            
            foreach (XmlNode xQuote in quoteList)
            {
                var quote = new Quote { QuoteText = xQuote.InnerText };
                var sourceUrl = xQuote.Attributes?.GetNamedItem("url");
                quote.Url = sourceUrl?.Value ?? "";
                entry.Quotes.Add(quote);
            }
        }
    }
}