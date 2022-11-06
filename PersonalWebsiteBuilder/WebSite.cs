using PersonalWebsiteBuilder.Widgets;

namespace PersonalWebsiteBuilder;

public class WebSite
{
    private readonly List<WebPage> _rootPages;
    public List<WidgetBase> Widgets { get; }
    internal string WebDesign { get; }
    public DirectoryInfo LocalSiteDirectory { get; }
    public DirectoryInfo StaticResourcesDirectory { get; }
    public string RemoteUrl { get; } 

    public WebSite(DirectoryInfo localSiteDirectory, DirectoryInfo staticResourcesDirectory, string remoteUrl)
    {
        _rootPages = new List<WebPage>();
        Widgets = new List<WidgetBase>();
        LocalSiteDirectory = localSiteDirectory;

        if (!LocalSiteDirectory.Exists)
            throw new SystemException("Local site directory not found.");

        StaticResourcesDirectory = staticResourcesDirectory;

        if (!StaticResourcesDirectory.Exists)
            throw new SystemException("Static resources directory not found.");

        var f = new FileInfo(System.Reflection.Assembly.GetEntryAssembly()!.Location);

        var designFile = new FileInfo(Path.Combine(f.Directory!.FullName, "webdesign.html"));

        WebDesign = File.ReadAllText(designFile.FullName);
        
        RemoteUrl = remoteUrl;

        if (!RemoteUrl.EndsWith("/"))
            RemoteUrl += "/";

        Widgets.Add(new StaticHtmlFromFile(this, "kaffe.txt"));
        Widgets.Add(new MenuWidget("Right", this, "card rightMenu"));
        Widgets.Add(new MenuWidget("Center", this, "card centerMenu"));
    }

    public void Render()
    {
        var all = GetAllPages();
        var widgetPositions = GetWidgetPositions();

        foreach (var webPage in all)
            webPage.Render(widgetPositions);
    }

    public List<WebPage> GetAllPages()
    {
        var result = new List<WebPage>();

        foreach (var rootPage in _rootPages)
        {
            result.Add(rootPage);
            result.AddRange(rootPage.GetChildPages());
        }

        return result;
    }

    public WebPage CreateRootPage(string htmlFileName, string longTitle, string shortTitle, string pageName)
    {
        var result = new WebPage(this, "", htmlFileName, longTitle, shortTitle, pageName)
        {
            ParentPage = null
        };

        _rootPages.Add(result);

        return result;
    }

    private List<string> GetWidgetPositions()
    {
        var result = new List<string>();

        var parts = WebDesign.Split("{Widgets:");

        foreach (var part in parts)
        {
            if (part == parts[0])
                continue;
            
            result.Add(part.Split("}")[0]);
        }

        return result;
    }

    public List<WidgetBase> GetWidgetsFor(string widgetPosition) =>
        Widgets.Where(x => x.Position == widgetPosition).ToList();
}