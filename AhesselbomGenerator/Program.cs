using System;

namespace AhesselbomGenerator;

public class Program
{
    [STAThread]
    private static void Main()
    {
        var source = new FileList();
        source.LoadFromPath(Config.SourceDirectory);
        foreach (var sourceFile in source)
        {
            if (
                !sourceFile.Name.EndsWith(".html")
                || sourceFile.Name.StartsWith("google")
                || sourceFile.Name == "data.html"
                || sourceFile.Name == "texts.html"
                || sourceFile.Name == ".htaccess"

            )
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(sourceFile.FullName);
                continue;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(sourceFile.FullName);
            var htmlProcessor = new HtmlProcessor(sourceFile.FullName);
            htmlProcessor.Process();
        }
    }
}