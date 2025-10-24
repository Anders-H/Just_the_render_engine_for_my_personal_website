using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AhesselbomGenerator.Menu;

public class MenuHtmlProcessor
{
    private readonly string _source;

    public MenuHtmlProcessor(string source)
    {
        _source = source;
    }

    public string GenerateMenu(string sourceDirectory)
    {
        var result = FileReader.GetTextFileContent(Path.Combine(sourceDirectory, "menu.txt"));

        if (_source.Contains(':'))
        {
            // Vi har hittat en submeny. Behåll förälderns submenyer.

            var arr = _source.Split(':');
            var parent = arr[0];
            result = new MenuProcessor(result).RemoveSubmenusExceptFor(parent);
        }
        else
        {
            // En huvudmeny. Behåll egna submenyer.
            result = new MenuProcessor(result)
                .RemoveSubmenusExceptFor(_source);
        }

        // Markera det egna menyalternativet.
        result = result.Replace($"<<{_source}>>", " selected");
        result = Regex.Replace(result, "<<[A-Za-z:]*>>", "");

        return result;
    }

    public string GenerateTopMenu(string sourceDirectory)
    {
        var menu = GenerateMenu(sourceDirectory);

        var menuRows = menu.Split(
            [Environment.NewLine],
            StringSplitOptions.None
        );

        var subItems = new List<string>();
        var s = new StringBuilder();
        s.Append(@"<div id=""topMenu"">");
        s.Append(@"<ul id=""topMenuUl"">");

        foreach (var row in menuRows)
        {
            if (row.IndexOf("&nbsp;", StringComparison.Ordinal) > 0)
            {
                subItems.Add(row);
                continue;
            }

            s.Append($"<li>{row}</li>");
        }

        s.Append("</ul>");
        s.Append("</div>");

        if (subItems.Count > 0)
        {
            s.Append(@"<div id=""topSubMenu"">");
            s.Append(@"<ul id=""subTopMenuUl"">");

            foreach (var subItem in subItems)
            {
                var i = subItem.Replace("&nbsp;", "");
                s.Append($"<li>{i}</li>");
            }

            s.Append("</ul>");
            s.Append("</div>");
        }

        return s.ToString();
    }
}