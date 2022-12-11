using System.Text;

namespace AhesselbomGenerator;

public class HeadGenerator
{
    private readonly int _folderDepth;

    public HeadGenerator(int folderDepth)
    {
        _folderDepth = folderDepth;
    }

    public string Generate(string title)
    {
        var level = "./";

        if (_folderDepth >= 1)
        {
            var s = new StringBuilder();

            for (var i = 0; i < _folderDepth; i++)
            {
                s.Append("../");
            }

            level = s.ToString();
        }

        return @$"<head>
<meta name=""viewport"" content=""width=device-width, initial-scale=1"">
<meta charset=""utf-8"" />
<title>{title}</title>
<link rel=""stylesheet"" href=""{level}style25.css"">
<script src=""{level}today.js""></script>
</head>";
    }
}