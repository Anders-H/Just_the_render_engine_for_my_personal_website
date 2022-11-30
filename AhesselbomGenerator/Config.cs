using System.IO;

namespace AhesselbomGenerator;

public class Config
{
    private Config()
    {
    }

    public static string SourceDirectory =>
        Directory.Exists("F:\\OneDrive\\ahesselbom.se2\\Source")
            ? "F:\\OneDrive\\ahesselbom.se2\\Source"
            : "C:\\OneDrive\\ahesselbom.se2\\Source";

    public static string Destination =>
        Directory.Exists("F:\\OneDrive\\ahesselbom.se2\\Output")
            ? "F:\\OneDrive\\ahesselbom.se2\\Output"
            : "C:\\OneDrive\\ahesselbom.se2\\Output";
}