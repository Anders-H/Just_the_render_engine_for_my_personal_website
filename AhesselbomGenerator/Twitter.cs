using System.Text;
using Microsoft.Data.SqlClient;

namespace AhesselbomGenerator;

public class Twitter
{
    public static string GetTweetHtml(bool skip5)
    {
        const string dSource = "Data Source=.";
        const string dName = "Initial Catalog=WebSiteTweetDatabase";
        const string iSecurity = "Integrated Security=True";
        const string tCert = "Trust Server Certificate=True";
        const string connectionString = $"{dSource};{dName};{iSecurity};{tCert}";

        var s = new StringBuilder();
        using var cn = new SqlConnection(connectionString);
        cn.Open();
        var cmd = new SqlCommand(skip5 ? "SELECT [Text], [Date], TweetLink FROM dbo.Tweet ORDER BY [Date] DESC OFFSET (5) ROWS FETCH NEXT (5) ROWS ONLY" : "SELECT TOP 5 [Text], [Date], TweetLink FROM dbo.Tweet ORDER BY [Date] DESC", cn);
        var r = cmd.ExecuteReader();

        s.Append(@"<table style=""border:1px solid #555555;background-color:#eeeeee;border-radius:8px;"">");
        s.Append("<tr>");
        s.Append(@"<th style=""font-weight:bold;text-align:center;vertical-align:top;"">Date</th>");
        s.Append(@"<th style=""font-weight:bold;text-align:center;vertical-align:top;"">Time</th>");
        s.Append(@"<th style=""font-weight:bold;text-align:left;vertical-align:top;""><a href=""https://twitter.com/ahesselbom"" target=""_blank"">Twitter/X</a></th>");
        s.Append("</tr>");

        while (r.Read())
        {
            var text = r.GetString(0);
            var date = r.GetDateTime(1);
            var link = r.GetString(2);
            s.Append("<tr>");
            s.Append($@"<td style=""text-align:center;font-size:smaller;white-space:nowrap;vertical-align:top;""><a href=""{link}"" target=""_blank"">{date:yyyy-MM-dd}</a></td>");
            s.Append($@"<td style=""text-align:center;font-size:smaller;white-space:nowrap;vertical-align:top;""><a href=""{link}"" target=""_blank"">{date:HH:mm}</a></td>");
            s.Append($@"<td style=""text-align:left;font-size:smaller;vertical-align:top;""><a href=""{link}"" target=""_blank"">{text}</a></td>");
            s.Append("</tr>");
        }

        s.Append("</table>");
        r.Close();
        cn.Close();
        return s.ToString();
    }

    public static string GetTweetHtmlTop100()
    {
        const string dSource = "Data Source=.";
        const string dName = "Initial Catalog=WebSiteTweetDatabase";
        const string iSecurity = "Integrated Security=True";
        const string tCert = "Trust Server Certificate=True";
        const string connectionString = $"{dSource};{dName};{iSecurity};{tCert}";

        var s = new StringBuilder();
        using var cn = new SqlConnection(connectionString);
        cn.Open();
        var cmd = new SqlCommand("SELECT TOP 100 [Text], [Date], TweetLink FROM dbo.Tweet ORDER BY [Date] DESC", cn);
        var r = cmd.ExecuteReader();

        s.Append(@"<table style=""border:none;"">");
        s.Append("<tr>");
        s.Append(@"<th style=""font-weight:bold;text-align:center;vertical-align:top;"">Date</th>");
        s.Append(@"<th style=""font-weight:bold;text-align:center;vertical-align:top;"">Time</th>");
        s.Append(@"<th style=""font-weight:bold;text-align:left;vertical-align:top;""><a href=""https://twitter.com/ahesselbom"" target=""_blank"">Twitter/X</a></th>");
        s.Append("</tr>");

        while (r.Read())
        {
            var text = r.GetString(0);
            var date = r.GetDateTime(1);
            var link = r.GetString(2);
            s.Append("<tr>");
            s.Append($@"<td style=""text-align:center;white-space:nowrap;vertical-align:top;""><a href=""{link}"" target=""_blank"">{date:yyyy-MM-dd}</a></td>");
            s.Append($@"<td style=""text-align:center;white-space:nowrap;vertical-align:top;""><a href=""{link}"" target=""_blank"">{date:HH:mm}</a></td>");
            s.Append($@"<td style=""text-align:left;vertical-align:top;""><a href=""{link}"" target=""_blank"">{text}</a></td>");
            s.Append("</tr>");
        }

        s.Append("</table>");
        r.Close();
        cn.Close();
        return s.ToString();
    }

    public static string GetLastTweet()
    {
        const string dSource = "Data Source=.";
        const string dName = "Initial Catalog=WebSiteTweetDatabase";
        const string iSecurity = "Integrated Security=True";
        const string tCert = "Trust Server Certificate=True";
        const string connectionString = $"{dSource};{dName};{iSecurity};{tCert}";

        using var cn = new SqlConnection(connectionString);
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