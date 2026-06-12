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

<nav>
    <div class=""logo"">https://ahesselbom.se/</div>
    <ul class=""nav-links"">
        <li><a href=""https://ahesselbom.se/om/"">Om</a></li>
        <li><a href=""https://ahesselbom.se/texter/"">Texter</a></li>
        <li><a class=""menuPrio1""href=""https://ahesselbom.se/youtube/"">YouTube</a></li>
        <li><a href=""https://ahesselbom.se/twitter/"">X</a></li>
        <li><a class=""menuPrio0""href=""https://ahesselbom.se/podcast/"">Podcasts</a></li>
        <li><a class=""menuPrio2""href=""https://ahesselbom.se/hall-of-fame/"">Hall of fame</a></li>
        <li><a class=""menuPrio3"" href=""https://ahesselbom.se/evolution/"">Evolution</a></li>
    </ul>
</nav>

    <header>
        <h1>Anders Hesselbom</h1>
        <p>Programmerare, skeptiker, sekulärhumanist, antirasist.<br/>Författare till bok om C64 och senbliven lantis. Röstar pirat.</p>
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
    const hiddenTeaser = document.querySelector('.hide-on-two');

    new ResizeObserver(() => {
        const cols = getComputedStyle(teaserGrid).gridTemplateColumns.split(' ').length;
        hiddenTeaser.style.display = cols === 2 ? 'none' : '';
    }).observe(teaserGrid);
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

        var comments = Template.Replace("[items]", cards)
            .Replace("[today]", GetToday())
            .Replace("[comments]", FileReader.GetTextFileContent(Path.Combine(Config.SourceDirectory, "comments.txt")));

        const string searchFor = "class=\"teaser endTeaser\"";
        const string replaceWith = "class=\"teaser endTeaser hide-on-two\"";

        var sistaIndex = comments.LastIndexOf(searchFor);

        if (sistaIndex == -1)
            return comments;

        var modified = comments[..sistaIndex] + replaceWith + comments[(sistaIndex + searchFor.Length)..];
        return modified;
    }

    private static string GetToday() =>
        @"<article class=""teaser endTeaser"">
            <h3>Idag</h3>
            <p>Om du läst ett bibelcitat på engelska och vill slå upp det på svenska, <a href=""https://politik-och-filosofi.ahesselbom.se/bibelns-bocker-pa-engelska/"">är det bra att veta vad motsvarande bok heter på svenska.</a></p>
            <p>Folkbildning om <a href=""https://ahesselbom.se/publicservice/"">public service samlas här.</a></p>
            <script>
                writeToday();
            </script>
        </article>";
}