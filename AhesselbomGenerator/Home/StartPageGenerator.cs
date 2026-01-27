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
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f9f9f9;
            color: #333;
            line-height: 1.6;
        }

        nav {
            background-color: #ffffff;
            height: 60px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 0 5%;
            position: sticky;
            top: 0;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            z-index: 1000;
        }

        .nav-links {
            list-style: none;
            display: flex;
            margin: 0;
            padding: 0;
        }

        .nav-links li {
            margin-left: 20px;
        }

        .nav-links a {
            text-decoration: none;
            color: #555;
            font-size: 0.9rem;
            transition: color 0.3s;
        }

        .nav-links a:hover {
            color: #007BFF;
        }

        footer a {
            text-decoration: none;
            color: #555;
            font-size: 0.9rem;
            transition: color 0.3s;
        }

        footer a:hover {
            color: #007BFF;
        }

        header {
            background-color: #ffffff;
            padding: 60px 20px;
            text-align: center;
            border-bottom: 1px solid #eee;
        }

        header h1 {
            margin: 0;
            font-size: 2.5rem;
            color: #222;
        }

        header p {
            font-size: 1.2rem;
            color: #666;
        }

        .hero-image {
            width: 100%;
            height: 400px;
            background-image: url('./img/start_bg.jpg');
            background-size: cover;
            background-position: center;
        }

        .container {
            max-width: 1000px;
            margin: 40px auto;
            padding: 0 20px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
        }
        .teaser {
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.15);
            transition: transform 0.4s;
            overflow: hidden;
            display: flex;
            flex-direction: column;
            height: 100%;
            box-sizing: border-box;
        }
        .teaser:hover {
            transform: translateY(-2px);
        }
        .teaser h3 {
            margin-top: 0;
            color: #444;
        }
        .teaser a {
            display: inline-block;
            margin-top: 10px;
            text-decoration: none;
            color: #007BFF;
            font-weight: bold;
            margin-top: auto;
            padding-top: 15px;
        }
        footer {
            text-align: center;
            padding: 40px;
            font-size: 0.9rem;
            color: #999;
        }

        .logo {
            cursor: default;
        }

        @media screen and (max-width: 730px) {
            .menuPrio3 {
                display: none;
            }
        }
        @media screen and (max-width: 660px) {
            .menuPrio2 {
                display: none;
            }
        }
        @media screen and (max-width: 540px) {
            .menuPrio1 {
                display: none;
            }
        }
        @media screen and (max-width: 440px) {
            .menuPrio0 {
                display: none;
            }
        }
    </style>
</head>
<body>

<nav>
    <div class=""logo"">https://ahesselbom.se/</div>
    <ul class=""nav-links"">
        <li><a href=""https://ahesselbom.se/om/"">Om</a></li>
        <li><a href=""https://ahesselbom.se/texter/"">Texter</a></li>
        <li><a href=""https://ahesselbom.se/youtube/"">YouTube</a></li>
        <li><a href=""https://ahesselbom.se/twitter/"">X</a></li>
        <li><a href=""https://ahesselbom.se/podcast/"">Podcasts</a></li>
        <li><a href=""https://ahesselbom.se/hall-of-fame/"">Hall of fame</a></li>
        <li><a href=""https://ahesselbom.se/evolution/"">Evolution</a></li>
    </ul>
</nav>

    <header>
        <h1>Anders Hesselbom</h1>
        <p>Programmerare, skeptiker, sekulärhumanist, antirasist. Författare till bok om C64 och senbliven lantis. Röstar pirat.</p>
    </header>

    <div class=""hero-image""></div>

    <div class=""container"">
[items]
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

</body>
</html>";

    public const string ItemTemplate = @"<article class=""teaser"">
        <h3>[A]</h3>
        <p>[B]</p>
        <a href=""[C]"">Läs mer</a>
    </article>";

    public static string GetStartPage(ISettings settings)
    {
        var cards = File.ReadAllText($"{settings.InputBasePath}start_cards.txt");
        return Template.Replace("[items]", cards);
    }
}