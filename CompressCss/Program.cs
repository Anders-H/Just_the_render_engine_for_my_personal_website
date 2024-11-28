using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

const string wrongNumberOfArguments = "Two arguments are required, input filename and output filename. The output file will be overwritten.";

if (args.Length != 2)
{
    Console.WriteLine(wrongNumberOfArguments);
    throw new SystemException(wrongNumberOfArguments);
}

var sourceFilename = args[0];
var targetFilename = args[1];
var targetFileInfo = new FileInfo(targetFilename);
var targetFullPath = targetFileInfo.FullName;
var sourceData = File.ReadAllText(sourceFilename);
var rows = sourceData.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
var targetData = new StringBuilder();
var replacements = new List<Replacement>();

foreach (var row in rows)
{
    var replacementMatch = Regex.Match(row, @"^\/\*\s*def\s+(.*)\s*=\s*(.*)\s*\*\/$");

    if (replacementMatch.Success)
    {
        RegisterReplacement(replacementMatch, replacements);
        continue;
    }

    targetData.AppendLine(UseReplacement(row, replacements));
}

File.WriteAllText(targetFullPath, targetData.ToString());

return;

static void RegisterReplacement(Match match, List<Replacement> rep)
{
    var source = match.Groups[1].Value;
    var target = match.Groups[2].Value;
    rep.Add(new Replacement(source, target));
}

static string UseReplacement(string source, List<Replacement> rep)
{
    foreach (var replacement in rep)
    {
        source = source.Replace(replacement.Source.Trim(), replacement.Target.Trim());
    }

    return source.Trim();
}

internal class Replacement
{
    public string Source { get; set; }
    public string Target { get; set; }

    public Replacement() : this("", "")
    {
    }

    public Replacement(string source, string target)
    {
        Source = source;
        Target = target;
    }
}