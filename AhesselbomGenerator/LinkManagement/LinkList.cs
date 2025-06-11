using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AhesselbomGenerator.LinkManagement;

public class LinkList : List<Link>
{
    private readonly string _sourceFilename;

    public LinkList(string sourceFilename)
    {
        _sourceFilename = sourceFilename;
    }

    public void Load()
    {
        Clear();

        var raw = FileReader.GetTextFileContent(_sourceFilename);

        var rows = Regex.Split(raw, @"\n");

        foreach (var row in rows)
        {
            var x = Link.Parse(row);
            if (x != null)
                Add(x);
        }
    }

    public string GenerateLinks()
    {
        var s = new StringBuilder();

        foreach (var link in this)
        {
            s.AppendLine(link.GenerateLink());

            if (link != this.Last())
                s.Append("<br>");
        }

        return s.ToString();
    }
}