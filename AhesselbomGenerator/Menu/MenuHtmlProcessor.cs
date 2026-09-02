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

                i = i.Replace("Blev det en klassiker?", "Klassiker?");
                i = i.Replace("Inte en singel", "Singel");
                i = i.Replace("Radio Houdi", "Houdi");
                i = i.Replace("Stulet index", "Stöldindex");
                i = i.Replace("Hesselbom/Sahlström", "Sahlström");

                s.Append($"<li>{i}</li>");
            }

            s.Append("</ul>");
            s.Append("</div>");
        }

        return s.ToString();
    }

    public string GenerateResponsiveTopMenu(string sourceDirectory, bool isHomePage = false)
    {
        var topMenu = FileReader.GetTextFileContent(Path.Combine(sourceDirectory, "top-menu.txt"));

        if (isHomePage)
        {
            topMenu = topMenu.Replace(
                @"<div class=""logo""><a href=""https://ahesselbom.se/"">Hem</a></div>",
                @"<div class=""logo"">https://ahesselbom.se/</div>");
        }

        topMenu = topMenu.Replace("<nav>", @"<nav aria-label=""Huvudmeny"">");

        var globalItems = ExtractGlobalMenuItems(topMenu);
        globalItems = MarkCurrentGlobalItem(globalItems, sourceDirectory);
        var localItems = ExtractLocalMenuItems(sourceDirectory);
        var mobileMenu = GenerateMobileMenu(globalItems, localItems);

        return topMenu.Replace("</nav>", $"{mobileMenu}{Environment.NewLine}</nav>");
    }

    private static string ExtractGlobalMenuItems(string topMenu)
    {
        var match = Regex.Match(
            topMenu,
            @"<ul class=""nav-links"">(?<items>.*?)</ul>",
            RegexOptions.Singleline);

        if (!match.Success)
            throw new InvalidOperationException("Could not find nav-links in top-menu.txt.");

        return Regex.Replace(
            match.Groups["items"].Value.Trim(),
            @"<li class=""menuPrio\d"">",
            "<li>");
    }

    private List<string> ExtractLocalMenuItems(string sourceDirectory)
    {
        if (string.IsNullOrWhiteSpace(_source))
            return [];

        var menuRows = Regex.Split(GenerateMenu(sourceDirectory), @"\r?\n");
        var result = new List<string>();

        foreach (var row in menuRows)
        {
            if (!row.Contains("&nbsp;", StringComparison.Ordinal))
                continue;

            var link = row.Replace("&nbsp;", "", StringComparison.Ordinal)
                .Replace(@" class=""menulink selected""", @" aria-current=""page""", StringComparison.Ordinal)
                .Replace(@" class=""menulink""", "", StringComparison.Ordinal)
                .Trim();

            result.Add($"<li>{link}</li>");
        }

        return result;
    }

    private string MarkCurrentGlobalItem(string globalItems, string sourceDirectory)
    {
        if (string.IsNullOrWhiteSpace(_source) || _source.Contains(':'))
            return globalItems;

        var marker = $"<<{_source}>>";
        var menuData = FileReader.GetTextFileContent(Path.Combine(sourceDirectory, "menu.txt"));

        foreach (var row in Regex.Split(menuData, @"\r?\n"))
        {
            if (!row.Contains(marker, StringComparison.Ordinal))
                continue;

            var hrefMatch = Regex.Match(row, @"href=""(?<href>[^""]+)""");

            if (!hrefMatch.Success)
                return globalItems;

            var href = hrefMatch.Groups["href"].Value;
            var markedItems = globalItems.Replace(
                $"href=\"{href}\"",
                $"href=\"{href}\" aria-current=\"page\"",
                StringComparison.Ordinal);

            if (markedItems != globalItems)
                return markedItems;

            var alternativeHref = href.EndsWith('/') ? href.TrimEnd('/') : $"{href}/";
            return globalItems.Replace(
                $"href=\"{alternativeHref}\"",
                $"href=\"{alternativeHref}\" aria-current=\"page\"",
                StringComparison.Ordinal);
        }

        return globalItems;
    }

    private string GenerateMobileMenu(string globalItems, IReadOnlyCollection<string> localItems)
    {
        var s = new StringBuilder();
        s.AppendLine(@"    <details class=""mobile-menu"">");
        s.AppendLine(@"        <summary><span aria-hidden=""true"">☰</span> Meny</summary>");
        s.AppendLine(@"        <div class=""mobile-menu-panel"">");
        s.AppendLine(@"            <div class=""mobile-menu-group"">");
        s.AppendLine(@"                <span class=""mobile-menu-heading"">Huvudmeny</span>");
        s.AppendLine("                <ul>");

        foreach (var row in Regex.Split(globalItems, @"\r?\n"))
            s.AppendLine($"                    {row.Trim()}");

        s.AppendLine("                </ul>");
        s.AppendLine("            </div>");

        if (localItems.Count > 0)
        {
            s.AppendLine(@"            <div class=""mobile-menu-group"">");
            s.AppendLine($@"                <span class=""mobile-menu-heading"">{GetLocalMenuHeading()}</span>");
            s.AppendLine("                <ul>");

            foreach (var item in localItems)
                s.AppendLine($"                    {item}");

            s.AppendLine("                </ul>");
            s.AppendLine("            </div>");
        }

        s.AppendLine("        </div>");
        s.Append("    </details>");
        return s.ToString();
    }

    private string GetLocalMenuHeading()
    {
        var parent = _source.Split(':')[0];

        return parent switch
        {
            "Home" => "Om webbplatsen",
            "Text" => "Texter",
            "YouTube" => "YouTube",
            "Podcast" => "Podcasts",
            "Evolution" => "Evolution",
            _ => "I den här avdelningen"
        };
    }
}
