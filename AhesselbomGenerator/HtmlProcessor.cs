using AhesselbomGenerator.Blogg;
using AhesselbomGenerator.HallOfFame;
using AhesselbomGenerator.LinkManagement;
using AhesselbomGenerator.Menu;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AhesselbomGenerator.Home;

namespace AhesselbomGenerator;

public class HtmlProcessor
{
    private string? _lastBlogAddress;
    private string? _lastBlogHeader;
    private string? _lastBlogShortText;
    private string? _lastTweet;
    private string Source { get; }
    private string Destination { get; set; }
    private static bool YouTubeSaved { get; set; }
    private static readonly ISettings Settings;
    public string Upload { get; private set; }

    static HtmlProcessor()
    {
        YouTubeSaved = false;
        Settings = new Settings();
    }

    public HtmlProcessor(string source)
    {
        Source = source;
        Destination = "";
        Upload = "";
    }

    public void Process()
    {
        if (Source.IndexOf(@"\publicservice\", StringComparison.Ordinal) > 5 && !Source.EndsWith(@"\publicservice\index.html"))
            return;

        var source = FileReader.GetTextFileContent(Source);
        var rows = Regex.Split(source, @"\n");

        var o = new StringBuilder();

        foreach (var row in rows)
        {
            var r = row;

            if (_lastBlogAddress == null)
                GetLastBlogAddress(out _lastBlogAddress, out _lastBlogHeader, out _lastBlogShortText);

            _lastTweet ??= GetLastTweet();

            r = r.Replace("LAST_BLOG_ADDRESS", _lastBlogAddress);
            r = r.Replace("LAST_BLOG_HEADER", _lastBlogHeader);
            r = r.Replace("LAST_BLOG_SHORT_TEXT", _lastBlogShortText);
            r = r.Replace("[&#8230;]", $@"<a href=""{_lastBlogAddress}"">[&#8230;]</a>");
            r = r.Replace("LAST_TWEET", _lastTweet);

            o.AppendLine(r.Trim().StartsWith("<!--") ? ProcessRow(r) : r);
        }

        if (string.IsNullOrWhiteSpace(Destination))
            throw new Exception("Missing destination.");

        var generated = o.ToString();

        Save(generated);
    }

    private string ProcessRow(string row)
    {
        string d;

        if (Directory.Exists(Config.Destination))
            d = Config.Destination;
        else
            throw new Exception("Primary destination not found, secondary destination not found.");

        var s = Config.SourceDirectory;

        row = row.Trim();

        if (!row.StartsWith("<!--"))
            return row;

        if (row.StartsWith("<!--HallOfFame:"))
        {
            var values = row.ExtractValues();
            new HallOfFameGenerator(values.Item1, values.Item2).Generate();
            return "";
        }

        if (row.StartsWith("<!--Output:"))
        {
            Destination = Path.Combine(d, row.ExtractValue());
            return "";
        }

        if (row.StartsWith("<!--Upload:"))
        {
            Upload = row.ExtractValue();
            return "";
        }

        if (row.StartsWith("<!--Embed:"))
        {
            var videoId = row.ExtractValue();
            return $@"
<div style=""width: 420px; height: 305px;"">
<iframe src=""https://www.youtube.com/embed/{videoId}?controls=0&showinfo=0&autoplay=0&loop=0&mute=0"" frameborder=""0"" style=""width: 420px; height: 290px;"" allowfullscreen></iframe>
</div>";
        }

        if (row.StartsWith("<!--LocalInstagram:"))
        {
            var x = row.ExtractValue();

            if (x.IndexOf('|') >= 0)
            {
                var parts = x.Split('|');
                return $@"<p class=""instaParagraph""><a href=""https://www.instagram.com/p/{parts[0]}/"" target=""_blank""><img src=""{parts[0]}.jpg"" alt=""{parts[1]}"" title=""{parts[1]}"" class=""instaImage""></a></p>";
            }

            return $@"<p class=""instaParagraph""><a href=""https://www.instagram.com/p/{x}/"" target=""_blank""><img src=""{x}.jpg"" class=""instaImage""></a></p>";
        }

        if (row.StartsWith("<!--LocalYouTube:"))
        {
            var x = row.ExtractValue();
            return $@"<p class=""instaParagraph"">
<a href=""https://www.youtube.com/watch?v={x}"" target=""_blank""><img src=""{x}.jpg""></a>
</p>";
        }

        if (row.StartsWith("<!--YouTubeList:"))
        {
            var youTube = new YouTubeListGenerator(row.ExtractValue()).Generate();

            if (!YouTubeSaved)
            {
                File.WriteAllText($"{Settings.OutputBasePath}rss\\veckanshesselbom_rss.xml", youTube.Rss);
                YouTubeSaved = true;
            }
                
            return youTube.Html;
        }

        if (row.StartsWith("<!--YouTubeLongList:"))
        {
            var youTube = new YouTubeListGenerator(row.ExtractValue()).GenerateLong();

            if (!YouTubeSaved)
            {
                File.WriteAllText($"{Settings.OutputBasePath}rss\\veckanshesselbom_rss.xml", youTube.Rss);
                YouTubeSaved = true;
            }

            return youTube.Html;
        }
        
        if (row.StartsWith("<!--YouTubeShortList:"))
        {
            var youTube = new YouTubeListGenerator(row.ExtractValue()).GenerateShort();
            return youTube.Html;
        }

        if (row.StartsWith("<!--BloggRss:"))
            return new BloggGenerator(row.ExtractValue()).Generate(true);

        if (row.StartsWith("<!--BloggRssTake5:"))
            return new BloggGenerator(row.ExtractValue(), 5).Generate(true);

        if (row.StartsWith("<!--StaticLink:"))
            return FileReader.GetTextFileContent(row.ExtractValue());

        if (row.StartsWith("<!--BloggRssHeaders:"))
            return new BloggGenerator(row.ExtractValue()).Generate(false);

        if (row.StartsWith("<!--BloggRssHeaders5:"))
            return new BloggGenerator(row.ExtractValue(), 5).Generate(false);

        if (row.StartsWith("<!--LastBlogHeader:"))
            return new BloggGenerator(row.ExtractValue()).GetLastBlogHeaderUrl();

        if (row.StartsWith("<!--BloggRssHeadersSkip5:"))
            return new BloggGenerator(row.ExtractValue(), 25).Generate(false, 5);

        if (row.StartsWith("<!--PodcastEpisodes:"))
            return new PodcastEpisodeListGenerator(row.ExtractValue()).Generate();

        if (row.StartsWith("<!--Breadcrumb:"))
            return new BreadcrumbGenerator(row.ExtractValue()).Generate();

        if (row.StartsWith("<!--Menu:"))
            return new MenuHtmlProcessor(row.ExtractValue()).GenerateMenu(s);

        if (row.StartsWith("<!--TopMenu:"))
            return new MenuHtmlProcessor(row.ExtractValue()).GenerateTopMenu(s);

        if (row.StartsWith("<!--Menu-->"))
        {
            var result = FileReader.GetTextFileContent(Path.Combine(s, "menu.txt"));
            result = new MenuProcessor(result).RemoveSubmenus();
            result = Regex.Replace(result, "<<[A-Za-z]*>>", "");
            return result;
        }

        if (row.StartsWith("<!--Menu26:"))
            return new StaticMenuProcessor(row.ExtractValue()).Generate();

        if (row.StartsWith("<!--Menu26-->"))
            return new StaticMenuProcessor("").Generate();

        if (row.StartsWith("<!--Include:"))
            return FileReader.GetTextFileContent(Path.Combine(s, row.ExtractValue()));

        if (row.StartsWith("<!--LinkList:"))
        {
            var linkList = new LinkList(Path.Combine(s, row.ExtractValue()));
            linkList.Load();
            return linkList.GenerateLinks();
        }

        if (row.StartsWith("<!--Head:"))
        {
            var x = row.Split(':');
            x[2] = x[2].Replace("-->", "");

            var headGenerator = new HeadGenerator(int.Parse(x[1]));
            return headGenerator.Generate(x[2], "style25.css");
        }

        if (row.StartsWith("<!--Head2024:"))
        {
            var x = row.Split(':');
            x[2] = x[2].Replace("-->", "");

            var headGenerator = new HeadGenerator(int.Parse(x[1]));
            return headGenerator.Generate2024(x[2]);
        }

        if (row.StartsWith("<!--GenerationDate"))
        {
            var n = DateTime.Now;
            var date = $"{n:yyyy:MM:dd} {n:HH:mm}";
            return $"<!-- Generated using the fourth generation of the Monkeybone engine at {date}, written by Anders Hesselbom. -->";
        }

        if (row.StartsWith("<!--Has:"))
        {
            var value = row.ExtractValue();
            return $"<!-- This row has: {value} -->";
        }

        if (row.StartsWith("<!--Twitter"))
        {
            if (row.StartsWith("<!--TwitterSkip5"))
                return Twitter.GetTweetHtml(true);

            if (row.StartsWith("<!--TwitterTop100Html"))
                return Twitter.GetTweetHtmlTop100();

            if (row.StartsWith("<!--TwitterTop100"))
            {
                var tweetRss = Twitter.GetTweetRssTop100();
                File.WriteAllText($"{Settings.OutputBasePath}rss\\ahesselbom_x_rss.xml", tweetRss);
                return tweetRss;
            }

            return Twitter.GetTweetHtml(false);
        }

        if (row == "<!--Home-->")
        {
            var home = StartPageGenerator.GetStartPage(Settings);
            File.WriteAllText($"{Settings.OutputBasePath}index.html", home);
            return home;
        }

        throw new SystemException(row);
    }

    public void Save(string content)
    {
        var flatContent = Regex.Split(content, @"\n");

        var s = new StringBuilder();

        foreach (var c in flatContent)
        {
            if (string.IsNullOrWhiteSpace(c))
                continue;

            s.AppendLine(c.TrimEnd());
        }

        var html = s.ToString().Trim();

        string oldHtml;

        try
        {
            oldHtml = File.ReadAllText(Destination, Encoding.UTF8).Trim();
        }
        catch
        {
            oldHtml = "";
        }

        if (oldHtml == html)
        {
            Console.WriteLine($"No need to save {Destination}.");
        }
        else
        {
            Console.WriteLine($"Saving {Destination}...");
            using var sw = new StreamWriter(Destination, false, Encoding.UTF8);
            sw.Write(html);
            sw.Flush();
            sw.Close();
        }
    }

    void GetLastBlogAddress(out string lastBlogAddress, out string lastBlogHeader, out string lastBlogShortText)
    {
        const string rssFilename = @"C:\Users\hbom\OneDrive\ahesselbom.se2\Output\rss\rss.xml";
        CheckFileIsXml(rssFilename);
        var bloggGenerator = new BloggGenerator(rssFilename);
        bloggGenerator.GetLast(out lastBlogAddress, out lastBlogHeader, out lastBlogShortText);
    }

    void CheckFileIsXml(string filename)
    {
        string content;
        
        using (var sr = new StreamReader(filename))
        {
            content = sr.ReadToEnd().Trim();
            sr.Close();
        }

        if (content.StartsWith("<?xml"))
            return;

        var xmlStart = content.IndexOf("<?xml", StringComparison.Ordinal);
        
        if (xmlStart == -1)
            throw new Exception($"File {filename} is not valid XML.");

        content = content[xmlStart..].Trim();
        
        using (var sw = new StreamWriter(filename, false, Encoding.UTF8))
        {
            sw.Write(content);
            sw.Flush();
            sw.Close();
        }
    }

    public string GetLastTweet()
    {
        return Twitter.GetLastTweet();
    }
}