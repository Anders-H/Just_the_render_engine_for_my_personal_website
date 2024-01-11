using System.Text;
using CommentCollector;

var inputFiles = new List<string>
{
    @"C:/Users/hbom/OneDrive/ahesselbom.se2/Output/rss/rss_comments.xml",
    @"C:/Users/hbom/OneDrive/ahesselbom.se2/Output/rss/winsoft-comments.xml"
};

const string outputFile = @"C:\Users\hbom\OneDrive\ahesselbom.se2\Source\comments.txt";

var options = new FileStreamOptions
{
    Access = FileAccess.Write,
    Mode = FileMode.Create
};

var commentList = new List<Comment>();

using var sw = new StreamWriter(outputFile, Encoding.UTF8, options);
sw.WriteLine(@"<div class=""card"">");

sw.WriteLine("</div>");