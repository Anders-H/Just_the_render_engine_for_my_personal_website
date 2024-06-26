using System.Text;

namespace AhesselbomGenerator;

public class HeadGenerator
{
    private readonly int _folderDepth;

    public HeadGenerator(int folderDepth)
    {
        _folderDepth = folderDepth;
    }

    public string Generate(string title, string css)
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
<meta http-equiv=""Cache-Control"" content=""no-cache, no-store, must-revalidate"" />
<meta http-equiv=""Pragma"" content=""no-cache"" />
<meta http-equiv=""Expires"" content=""Tue, 01 Jan 1980 1:00:00 GMT"" />
<title>{title}</title>
<link rel=""stylesheet"" href=""{level}{css}"">
<script src=""{level}today.js""></script>
</head>";
    }
}