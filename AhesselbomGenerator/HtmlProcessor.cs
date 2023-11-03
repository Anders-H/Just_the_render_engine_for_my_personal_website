using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AhesselbomGenerator.HallOfFame;
using AhesselbomGenerator.LinkManagement;

namespace AhesselbomGenerator;

public class HtmlProcessor
{
    private string Source { get; }
    private string Destination { get; set; }
    public string Upload { get; private set; }

    public HtmlProcessor(string source)
    {
        Source = source;
        Destination = "";
        Upload = "";
    }

    public void Process()
    {
        var source = FileReader.GetTextFileContent(Source);
        var rows = Regex.Split(source, @"\n");
        
        var o = new StringBuilder();

        foreach (var row in rows)
            o.AppendLine(row.Trim().StartsWith("<!--") ? ProcessRow(row) : row);
        
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
            return $@"<p class=""instaParagraph"">
<a href=""https://www.instagram.com/p/{x}/"" target=""_blank""><img src=""{x}.jpg"" class=""instaImage"" /></a>
</p>";
        }
            
        if (row.StartsWith("<!--LocalYouTube:"))
        {
            var x = row.ExtractValue();
            return $@"<p class=""instaParagraph"">
<a href=""https://www.youtube.com/watch?v={x}"" target=""_blank""><img src=""{x}.jpg"" style="""" /></a>
</p>";
        }
            
        if (row.StartsWith("<!--YouTubeList:"))
            return new YouTubeListGenerator(row.ExtractValue()).Generate();
            
        if (row.StartsWith("<!--BloggRss:"))
            return new BloggGenerator(row.ExtractValue()).Generate(true);

        if (row.StartsWith("<!--BloggRssTake5:"))
            return new BloggGenerator(row.ExtractValue(), 5).Generate(true);

        if (row.StartsWith("<!--StaticLink:"))
            return FileReader.GetTextFileContent(row.ExtractValue());
            
        if (row.StartsWith("<!--BloggRssHeaders:"))
            return new BloggGenerator(row.ExtractValue()).Generate(false);

        if (row.StartsWith("<!--BloggRssHeadersSkip5:"))
            return new BloggGenerator(row.ExtractValue(), 5).Generate(false, 5);

        if (row.StartsWith("<!--PodcastEpisodes:"))
            return new PodcastEpisodeListGenerator(row.ExtractValue()).Generate();

        if (row.StartsWith("<!--Breadcrumb:"))
            return new BreadcrumbGenerator(row.ExtractValue()).Generate();

        if (row.StartsWith("<!--Menu:"))
        {
            var v = row.ExtractValue();
            var result = FileReader.GetTextFileContent(Path.Combine(s, "menu.txt"));

            if (v.Contains(':'))
            {
                // Vi har hittat en submeny. Behåll förälderns submenyer.

                var arr = v.Split(':');
                var parent = arr[0];

                result = new MenuProcessor(result)
                    .RemoveSubmenusExceptFor(parent);
            }
            else
            {
                // En huvudmeny. Behåll egna submenyer.
                result = new MenuProcessor(result)
                    .RemoveSubmenusExceptFor(v);
            }

            // Markera det egna menyalternativet.
            result = result.Replace($"<<{v}>>", " selected");
            result = Regex.Replace(result, "<<[A-Za-z:]*>>", "");

            return result;
        }
            
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

        if (row.StartsWith("<!--GenerationDate"))
        {
            var n = DateTime.Now;
            var date = $@"{n:yyyy:MM:dd} {n:hh:mm}";
            return $@"<!-- Generated using the third generation of the Monkeybone engine at {date}. -->";
        }

        if (row.StartsWith("<!--Has:"))
        {
            var value = row.ExtractValue();
            return $@"<!-- This row has: {value} -->";
        }

        throw new SystemException(row);
    }

    private void Save(string content)
    {
        var flatContent = Regex.Split(content, @"\n");
            
        var s = new StringBuilder();
            
        foreach (var c in flatContent)
        {
            if (string.IsNullOrWhiteSpace(c))
                continue;

            s.AppendLine(c.TrimEnd());
        }

        var html = s.ToString();

        if (html.IndexOf("ooooo", StringComparison.Ordinal) > 0)
            Console.WriteLine(html);

        using var sw = new StreamWriter(Destination, false, Encoding.UTF8);
        sw.Write(html);
        sw.Write("WHAT IS THIS!!!!");
        sw.Flush();
        sw.Close();
    }
}