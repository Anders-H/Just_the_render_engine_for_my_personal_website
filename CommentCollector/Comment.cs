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
        $@"<p><a href=""{Link}"">{HttpUtility.HtmlEncode(Creator)} ({PublishedTime:yyyy-MM-dd HH:mm})</a></p>{Content}";
}