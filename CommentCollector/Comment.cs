using System.Web;

namespace CommentCollector;

public class Comment
{
    public string Title { get; }
    public string Link { get; }
    public DateTime PublishedTime { get; }
    public string Creator { get; }
    public string Content { get; }

    public Comment(string title, string link, DateTime publishedTime, string creator, string content)
    {
        Title = title;
        Link = link;
        PublishedTime = publishedTime;
        Creator = creator;
        Content = content;
    }

    public override string ToString() =>
        $"{Title} ({PublishedTime:yyyy-MM-dd hh:mm:ss})";

    public string ToHtml() =>
        $@"<p><a href=""{HttpUtility.HtmlEncode(Link)}"">{HttpUtility.HtmlEncode(Creator)} ({PublishedTime:yyyy-MM-dd HH:mm})</a></p>{Content}";

    public string ToSidebarHtml() =>
        $@"    <article class=""comment"">
        <p class=""comment-meta"">
            <a href=""{HttpUtility.HtmlEncode(Link)}"">{HttpUtility.HtmlEncode(Creator)}</a>
            <time datetime=""{PublishedTime:yyyy-MM-ddTHH:mm}"">{PublishedTime:yyyy-MM-dd HH:mm}</time>
        </p>
        <div class=""comment-text"">{Content}</div>
    </article>";
}
