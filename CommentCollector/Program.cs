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

foreach (var inputFile in inputFiles)
    commentList.AddRange(GetComments(inputFile));

var sortedComments = commentList.OrderByDescending(x => x.PublishedTime).ToList();

using var sw = new StreamWriter(outputFile, Encoding.UTF8, options);
sw.WriteLine(@"<div class=""card"">");
sw.WriteLine("<h3>Kommentarer</h3>");
var count = 0;

foreach (var comment in sortedComments)
{
    count++;

    sw.WriteLine(comment);

    if (count > 10)
        break;
}

sw.WriteLine("</div>");

return;

List<Comment> GetComments(string filePath)
{

}