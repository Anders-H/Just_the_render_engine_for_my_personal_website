using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AhesselbomGenerator.HallOfFame;

public class HallOfFameGenerator
{
    private readonly string _sourcePath;
    private readonly string _targetPath;

    public HallOfFameGenerator(string sourcePath, string targetPath)
    {
        _sourcePath = sourcePath;
        _targetPath = targetPath;
    }

    public void Generate()
    {
        var l = new HallOfFameEntryList();
        l.Load(_sourcePath);

        using var x = new StreamWriter(_targetPath, false, Encoding.UTF8);

        foreach (var item in l)
        {
            x.WriteLine();

            if (l.First() != item)
                x.WriteLine("<p><br /></p>");

            x.Write($@"<p style=""text-align: center; text-transform: uppercase;""><b>{HttpUtility.HtmlEncode(item.Name)}</b></p>");

            x.Write($@"<p style=""text-align: center;"">{item.Cv}</p>");

            x.Write($@"<p style=""margin: 0; padding: 0; text-align: center;""><img src=""{item.Image}"" alt=""{item.Name}"" style=""width: 200px; height: 266px;""></p>");

            if (!string.IsNullOrEmpty(item.ImageCredit) && !string.IsNullOrEmpty(item.ImageCreditUrl))
                x.Write($@"<p style=""text-align: center; font-size: smaller;"">Foto: <a href=""{item.ImageCreditUrl}"" target=""_blank"">{item.ImageCredit}</a></p>");
            else if (!string.IsNullOrEmpty(item.ImageCredit))
                x.Write($@"<p style=""text-align: center; font-size: smaller;"">Foto: {item.ImageCredit}</p>");

            foreach (var quote in item.Quotes)
                x.Write(string.IsNullOrEmpty(quote.Url)
                    ? $"<p>&quot;{quote.QuoteText}&quot;</p>"
                    : $@"<p>&quot;<a href=""{quote.Url}"" target=""_blank"">{quote.QuoteText}</a>&quot;</p>");

            x.WriteLine();
        }
        x.Flush();
        x.Close();
    }
}