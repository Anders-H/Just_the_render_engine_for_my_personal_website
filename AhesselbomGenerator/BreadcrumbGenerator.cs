using System.ComponentModel.Design;
using System.Text;
using System.Text.RegularExpressions;

namespace AhesselbomGenerator;

public class BreadcrumbGenerator
{
    private readonly string _source;

    public BreadcrumbGenerator(string source)
    {
        _source = source;
    }

    public string Generate()
    {
        var result = new StringBuilder();
        var parts = _source.Split("][");

        for (var i = 0; i < parts.Length; i++)
        {
            var p = parts[i];

            if (p.StartsWith('['))
                p = p.Substring(1);

            if (p.EndsWith("]"))
                p = p.Substring(0, p.Length - 1);

            if (p.IndexOf('|') > -1)
            {
                var linkParts = p.Split("|");
                var url = linkParts[0].Replace("§", @"https://ahesselbom.se/");
                var text = linkParts[1];

                result.Append($@"<a href=""{url}"" class=""breadcrumLink"">{text}</a>");
            }
            else
            {
                result.Append($@"<span class=""breadcrumText"">{p}</span>");
            }

            if (i < parts.Length - 1)
                result.Append(@"&nbsp;<span class=""breadcrumArrow"">&nbsp;▸&nbsp;</span>&nbsp;");
        }

        return result.ToString();
    }
}