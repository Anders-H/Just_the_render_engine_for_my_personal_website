using System.IO;
using System.Net;
using System.Text;

namespace AhesselbomGenerator;

public class FileReader
{
    private static string? _menu;

    private FileReader()
    {
    }

    public static string GetTextFileContent(string filename)
    {
        if (filename.EndsWith(@"\menu.txt") && !string.IsNullOrWhiteSpace(_menu))
            return _menu;

        using var sr = new StreamReader(filename, Encoding.UTF8);
        var ret = sr.ReadToEnd();
        sr.Close();

        if (filename.EndsWith(@"\menu.txt"))
            _menu = ret;

        return ret;
    }

    public static void DownloadTextFile(string source, string target)
    {
        using var client = new WebClient();
        client.DownloadFile(source, target);
    }
}