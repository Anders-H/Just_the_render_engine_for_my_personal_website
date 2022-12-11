using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using AhesselbomGenerator.Xml;

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

        foreach (XmlNode x in dom.DocumentElement)
        {
            var xCredit = x.SelectNode("imagecredit");
            var xCreditUrl = xCredit.GetAttributeValue("url");
            
            var entry = new HallOfFameEntry(
                x.GetText("name"),
                x.GetText("cv"),
                x.GetText("image"),
                xCredit?.InnerText ?? "",
                xCreditUrl,
                x.GetText("page")
            );
            
            Add(entry);
            
            var quoteList = x.SelectNode("quotes")?.SelectNodes("quote");
            
            if (quoteList == null)
                continue;
            
            foreach (XmlNode xQuote in quoteList)
                entry.Quotes.Add(new Quote(xQuote.InnerText, xQuote.GetAttributeValue("url")));
        }
    }
}