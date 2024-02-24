using System.Diagnostics;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using MrTweet;

const string dSource = "Data Source=.";
const string dName = "Initial Catalog=WebSiteTweetDatabase";
const string iSecurity = "Integrated Security=True";
const string tCert = "Trust Server Certificate=True";
const string connectionString = $"{dSource};{dName};{iSecurity};{tCert}";
using var cn = new SqlConnection(connectionString);
cn.Open();
using var cmdGetNewestTweets = new SqlCommand("SELECT TOP 3 [Text] FROM dbo.Tweet ORDER BY [Date] DESC", cn);
var r = cmdGetNewestTweets.ExecuteReader();

while (r.Read())
{
    Console.WriteLine(r.GetString(0));
    Console.WriteLine();
}

r.Close();

Console.Write("Tweet: ");
var text = (Console.ReadLine() ?? "").Trim();

if (string.IsNullOrEmpty(text))
    return;

Console.Write("Date (YYYY-MM-DD, default is today): ");
var date = (Console.ReadLine() ?? "").Trim();

if (string.IsNullOrEmpty(date))
    date = DateTime.Now.ToString("yyyy-MM-dd");

var dateParts = date.Split('-');
var dateYear = int.Parse(dateParts[0]);
var dateMonth = int.Parse(dateParts[1]);
var dateDay = int.Parse(dateParts[2]);

Console.Write("Time (HH:MM, default is now): ");
var time = (Console.ReadLine() ?? "").Trim();

if (string.IsNullOrWhiteSpace(time))
    time = DateTime.Now.ToString("hh:mm");

Console.Write("Link to tweet: ");
var tweetLink = (Console.ReadLine() ?? "").Trim();

if (string.IsNullOrWhiteSpace(tweetLink))
    return;

var timeParts = time.Split(":");
var timeHour = int.Parse(timeParts[0]);
var timeMinute = int.Parse(timeParts[1]);

var tweetDate = new DateTime(dateYear, dateMonth, dateDay, timeHour, timeMinute, 0);

Console.WriteLine($"Tweet date/time: {tweetDate:yyyy-MM-dd hh:mm}");

Console.Write("Save (Y/N)? ");
var save = (Console.ReadLine() ?? "").ToLower();

if (save != "y")
    return;

using var cmdSave = new SqlCommand(@"
INSERT INTO dbo.Tweet (
    [Text], [Date], TweetLink
)
VALUES (
    @Text, @Date, @TweetLink
)", cn);

cmdSave.Parameters.AddWithValue("@Text", text);
cmdSave.Parameters.AddWithValue("@Date", tweetDate);
cmdSave.Parameters.AddWithValue("@TweetLink", tweetLink);
cmdSave.ExecuteNonQuery();

cn.Close();

Console.WriteLine("Generating https://ahesselbom.se/...");
var renderProcess = Process.Start(@"D:\GitRepos\Just_the_render_engine_for_my_personal_website\AhesselbomGenerator\bin\Release\net8.0-windows\AhesselbomGenerator.exe");
renderProcess.WaitForExit();
renderProcess.Dispose();

Console.WriteLine("Uploading...");
ISecrets s = new MySecrets();
var uploadProcessInfo = new ProcessStartInfo(@"D:\GitRepos\FtpMultiUpload\FtpMultiUpload\bin\Release\net8.0-windows\FtpMultiUpload.exe")
{
    Arguments = $@"{s.Target} {s.Username} {s.Password} ""{s.OutputPath}"" ""{s.OutputLog}"""
};
var uploadProcess = Process.Start(uploadProcessInfo);
uploadProcess!.WaitForExit();
uploadProcess!.Dispose();

Console.WriteLine("Happy computer, good computer! Bye!");

public interface ISecrets
{
    string Target { get; }
    string Username { get; }
    string Password { get; }
    string OutputPath { get; }
    string OutputLog { get; }
}