using System;
using System.IO;

namespace AhesselbomGenerator.Home;

public class StartPageGenerator
{
    public const string Template = @"<!DOCTYPE html>
<html lang=""sv"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Anders Hesselbom</title>
    <script src=""./today.js""></script>
    <link rel=""stylesheet"" href=""./style5.css"">
</head>
<body>

[top-menu]

    <header>
        <h1>Anders Hesselbom</h1>
        <p class=""home-tagline"">Programmerare, skeptiker, sekulärhumanist, antirasist.<br/>Författare till bok om C64 och senbliven lantis. Röstar pirat.</p>
    </header>

    <div class=""hero-image""></div>

    <div class=""container"">
[items]
[today]
[comments]
    </div>
    <footer>
        <a href=""https://ahesselbom.se/"">Hem</a>
        <span>&nbsp;|&nbsp;</span>
        <a href=""https://linktr.ee/hesselbom"" target=""_blank"">linktr.ee/hesselbom</a>
        <span>&nbsp;|&nbsp;</span>
        <a href=""https://winsoft.se/"">winsoft.se</a>
        <span>&nbsp;|&nbsp;</span>
        <a href=""http://80tal.se/"" target=""_blank"">80tal.se</a>
        <span>&nbsp;|&nbsp;</span>
        <a href=""https://filmtips.winsoft.se/"" target=""_blank"">Filmtips</a>
    </footer>
    <script>
        const teaserGrid = document.querySelector('.container');
        const endTeaser = teaserGrid?.querySelector('.endTeaser');

        if (teaserGrid && endTeaser) {
            const updateEndTeaserVisibility = () => {
                const columnCount = getComputedStyle(teaserGrid)
                    .gridTemplateColumns
                    .split(/\s+/)
                    .filter(Boolean)
                    .length;
                const teaserCount = teaserGrid.querySelectorAll('.teaser').length;

                const isAloneOnLastRow = columnCount > 1 && teaserCount % columnCount === 1;
                endTeaser.style.display = isAloneOnLastRow ? 'none' : '';
            };

            new ResizeObserver(updateEndTeaserVisibility).observe(teaserGrid);
            updateEndTeaserVisibility();
        }
    </script>
</body>
</html>";

    public const string ItemTemplate = @"<article class=""teaser"">
        <h3>[A]</h3>
        <p>[B]</p>
        <a href=""[C]""[D]>Läs mer</a>
    </article>";

    public static string GetStartPage(ISettings settings)
    {
        var cards = File.ReadAllText($"{settings.InputBasePath}start_cards.txt");

        var page = Template.Replace("[items]", cards)
            .Replace("[top-menu]", new global::AhesselbomGenerator.Menu.MenuHtmlProcessor("").GenerateResponsiveTopMenu(Config.SourceDirectory, true))
            .Replace("[today]", GetToday())
            .Replace("[comments]", FileReader.GetTextFileContent(Path.Combine(Config.SourceDirectory, "comments-home.txt")));

        return MarkLastTeaser(page);
    }

    private static string MarkLastTeaser(string page)
    {
        page = page.Replace("class=\"teaser endTeaser", "class=\"teaser");

        const string teaserStart = "<article class=\"teaser";
        var lastTeaser = page.LastIndexOf(teaserStart, StringComparison.Ordinal);

        if (lastTeaser < 0)
            return page;

        var classEnd = page.IndexOf('"', lastTeaser + teaserStart.Length);

        return classEnd < 0
            ? page
            : page.Insert(classEnd, " endTeaser");
    }

    private static string GetToday() =>
        @"<article class=""teaser idagTeaser"">
            <h3>Idag</h3>
            <p>Om du läst ett bibelcitat på engelska och vill slå upp det på svenska, <a href=""https://politik-och-filosofi.ahesselbom.se/bibelns-bocker-pa-engelska/"">är det bra att veta vad motsvarande bok heter på svenska.</a></p>
            <p>Folkbildning om <a href=""https://ahesselbom.se/publicservice/"">public service samlas här.</a></p>
            <script>
                writeToday();
            </script>
        </article>";
}
