using System.IO;
using System.Net;
using System.Text;

namespace AhesselbomGenerator;

public class FileReader
{
    private FileReader()
    {
    }

    public static string GetTextFileContent(string filename)
    {
        using var sr = new StreamReader(filename, Encoding.UTF8);
        var ret = sr.ReadToEnd();
        sr.Close();
        return ret;
    }

    public static void DownloadTextFile(string source, string target)
    {
        using var client = new WebClient();
        client.DownloadFile(source, target);
    }
}