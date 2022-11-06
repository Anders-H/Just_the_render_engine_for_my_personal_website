using System.Text;

namespace PersonalWebsiteBuilder;

public class WebPage
{
    private readonly WebSite _webSite;
    private readonly List<WebPage> _childPages;
    public string WebFolderName { get; internal set; }
    public string HtmlFileName { get; internal set; }
    public string LongTitle { get; internal set; }
    public string ShortTitle { get; internal set; }
    public string PageName { get; }
    public WebPage? ParentPage { get; set; }

    internal WebPage(WebSite webSite, string webFolderName, string htmlFileName, string longTitle, string shortTitle, string pageName)
    {
        _childPages = new List<WebPage>();
        _webSite = webSite;
        WebFolderName = webFolderName;
        HtmlFileName = htmlFileName;
        LongTitle = longTitle;
        ShortTitle = shortTitle;
        PageName = pageName;
    }

    internal void Render(List<string> widgetPositions)
    {
        var file = new FileInfo(GetLocalPath());

        if (!file.Directory!.Exists)
            file.Directory.Create();

        var html = _webSite.WebDesign;

        html = html.Replace("{PathDepth}", GetRemoteStepupToRoot());
        html = html.Replace("{PathMenu}", GetPathMenu());

        foreach (var widgetPosition in widgetPositions)
        {
            var widgets = _webSite.GetWidgetsFor(widgetPosition);
            var s = new StringBuilder();

            foreach (var widget in widgets)
                s.Append(widget.Render(this));

            html = html.Replace($"{{Widgets:{widgetPosition}}}", s.ToString());
        }

        using var stream = new StreamWriter(file.FullName);
        stream.Write(html);
        stream.Flush();
        stream.Close();
    }

    private string GetPathMenu()
    {
        var s = ShortTitle;

        var parent = ParentPage;

        while (parent != null)
        {
            s = parent.GetShortLink() + " ▶ " + s;
            parent = parent.ParentPage;
        }

        return s;
    }

    private string GetRemoteUrl()
    {
        var s = HtmlFileName == "index.html" ? "" : HtmlFileName;

        if (!string.IsNullOrEmpty(WebFolderName))
            s = WebFolderName + "/" + s;

        var parent = ParentPage;

        while (parent != null)
        {
            if (!string.IsNullOrEmpty(parent.WebFolderName))
                s = parent.WebFolderName + "/" + s;

            parent = parent.ParentPage;
        }

        return _webSite.RemoteUrl + s;
    }

    public string GetShortLink() =>
        $@"<a href=""{GetRemoteUrl()}"">{ShortTitle}</a>";

    public List<WebPage> GetChildPages()
    {
        var result = new List<WebPage>();

        foreach (var childPage in _childPages)
        {
            result.Add(childPage);
            result.AddRange(childPage.GetChildPages());
        }

        return result;
    }

    public WebPage CreateChildPage(string webFolderName, string htmlFileName, string longTitle, string shortTitle, string pageName)
    {
        var result = new WebPage(_webSite, webFolderName, htmlFileName, longTitle, shortTitle, pageName)
        {
            ParentPage = this
        };

        _childPages.Add(result);

        return result;
    }

    public int GetDepth()
    {
        var depth = 1;

        var parent = ParentPage;

        while (parent != null)
        {
            depth++;
            parent = parent.ParentPage;
        }

        return depth;
    }

    public string GetRemoteStepupToRoot()
    {
        var d = GetDepth();

        if (d <= 1)
            return "./";

        var s = new StringBuilder();

        for (var i = 1; i < d; i++)
            s.Append("../");

        return s.ToString();
    }

    public string GetLocalPath()
    {
        var path = WebFolderName;

        var parent = ParentPage;

        while (parent != null)
        {
            if (path == "")
                path = parent.WebFolderName;
            else if (parent.WebFolderName != "")
                path = parent.WebFolderName + @"\" + path;

            parent = parent.ParentPage;
        }

        var result = path == "" ? _webSite.LocalSiteDirectory.FullName : Path.Combine(_webSite.LocalSiteDirectory.FullName, path);

        return Path.Combine(result, HtmlFileName);
    }

    public string GetRemotePath()
    {
        var path = WebFolderName;

        var parent = ParentPage;

        while (parent != null)
        {
            if (path == "")
                path = parent.WebFolderName;
            else
                path = parent.WebFolderName + "/" + path;

            parent = parent.ParentPage;
        }

        return path + "/" + HtmlFileName;
    }
}