using System.IO;

namespace AhesselbomGenerator;

public class Config
{
    private Config()
    {
    }

    public static string SourceDirectory =>
        @"C:\Users\hbom\OneDrive\ahesselbom.se2\Source";

    public static string Destination =>
        @"C:\Users\hbom\OneDrive\ahesselbom.se2\Output";
}