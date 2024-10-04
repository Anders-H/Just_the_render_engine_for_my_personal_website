using System.Drawing.Text;
using System.Text;
using Microsoft.Data.SqlClient;

namespace AhesselbomGenerator;

public class Twitter
{
    private const string bold = "font-weight:bold;";
    private const string left = "text-align:left;vertical-align:top;";
    private const string center = "text-align:center;vertical-align:top;";
    public static string GetTweetHtml(bool skip5)
    {
        var s = new StringBuilder();
        using var cn = new SqlConnection(ConnectionStringBuilder.ConnectionString);
        cn.Open();
        const string fields = "[Date], [Text], TweetLink";
        const string skip5Sql = $"SELECT {fields} FROM dbo.Tweet ORDER BY [Date] DESC OFFSET (5) ROWS FETCH NEXT (5) ROWS ONLY";
        const string take5Sql = $"SELECT TOP 5 {fields} FROM dbo.Tweet ORDER BY [Date] DESC";
        var cmd = new SqlCommand(skip5 ? skip5Sql : take5Sql, cn);
        var r = cmd.ExecuteReader();

        s.Append(@"<table style=""border:1px solid #555555;background-color:#eeeeee;border-radius:8px;"">");
        s.Append("<tr>");
        s.Append($@"<th style=""{bold}{center}"">Date</th>");
        s.Append($@"<th style=""{bold}{center}"">Time</th>");
        s.Append($@"<th style=""{bold}{left}""><a href=""https://x.com/ahesselbom"" target=""_blank"">X (Twitter)</a></th>");
        s.Append("</tr>");

        while (r.Read())
        {
            var date = r.GetDateTime(0);
            var text = r.GetString(1);
            var link = r.GetString(2);
            s.Append("<tr>");
            s.Append($@"<td style=""{center}white-space:nowrap;font-size:smaller;""><a href=""{link}"" target=""_blank"">{date:yyyy-MM-dd}</a></td>");
            s.Append($@"<td style=""{center}white-space:nowrap;font-size:smaller;""><a href=""{link}"" target=""_blank"">{date:HH:mm}</a></td>");
            s.Append($@"<td style=""{left}font-size:smaller;""><a href=""{link}"" target=""_blank"">{text}</a></td>");
            s.Append("</tr>");
        }

        s.Append("</table>");
        r.Close();
        cn.Close();
        return s.ToString();
    }

    public static string GetTweetHtmlTop100()
    {
        var s = new StringBuilder();
        using var cn = new SqlConnection(ConnectionStringBuilder.ConnectionString);
        cn.Open();
        var cmd = new SqlCommand("SELECT TOP 100 [Date], [Text], TweetLink FROM dbo.Tweet ORDER BY [Date] DESC", cn);
        var r = cmd.ExecuteReader();

        s.Append(@"<table style=""border:none;"">");
        s.Append("<tr>");
        s.Append($@"<th style=""{bold}{center}"">Date</th>");
        s.Append($@"<th style=""{bold}{center}"">Time</th>");
        s.Append($@"<th style=""{bold}{left}""><a href=""https://x.com/ahesselbom"" target=""_blank"">X (Twitter)</a></th>");
        s.Append("</tr>");

        while (r.Read())
        {
            var date = r.GetDateTime(0);
            var text = r.GetString(1);
            var link = r.GetString(2);
            s.Append("<tr>");
            s.Append($@"<td style=""{center}white-space:nowrap;""><a href=""{link}"" target=""_blank"">{date:yyyy-MM-dd}</a></td>");
            s.Append($@"<td style=""{center}white-space:nowrap;""><a href=""{link}"" target=""_blank"">{date:HH:mm}</a></td>");
            s.Append($@"<td style=""{left}""><a href=""{link}"" target=""_blank"">{text}</a></td>");
            s.Append("</tr>");
        }

        s.Append("</table>");
        r.Close();
        cn.Close();
        return s.ToString();
    }

    public static string GetLastTweet()
    {
        using var cn = new SqlConnection(ConnectionStringBuilder.ConnectionString);
        cn.Open();
        var cmd = new SqlCommand("SELECT TOP 1 [Text] FROM dbo.Tweet ORDER BY [Date] DESC", cn);
        var r = cmd.ExecuteReader();
        var result = "";
        
        if (r.Read())
            result = r.GetString(0);

        r.Close();
        cn.Close();
        return result;
    }
}