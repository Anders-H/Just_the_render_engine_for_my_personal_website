using System.Linq;
using System.Text;
using MutableStringLibrary;

namespace AhesselbomGenerator;

public class MenuProcessor
{
    private readonly string _menuData;

    public MenuProcessor(string menuData)
    {
        _menuData = menuData;
    }

    public string RemoveSubmenus()
    {
        var s = new MutableString(_menuData);
        var rows = s.Rows();
        rows.RemoveIf(IsSubmenu);
        var result = new StringBuilder();

        foreach (var row in rows)
            result.AppendLine(row.Value);

        return result.ToString();
    }

    public string RemoveSubmenusExceptFor(string parent)
    {
        var s = new MutableString(_menuData);
        var rows = s.Rows();
        rows.RemoveIf(x => IsSubmenu(x) && !ParentNameIs(x, parent));
        var result = new StringBuilder();

        foreach (var row in rows)
            result.AppendLine(row.Value);

        return result.ToString();
    }

    private static bool IsSubmenu(MutableString s) =>
        s.Value != null
        && s.Value.Count(x => x == ':') == 2;

    private static bool ParentNameIs(MutableString s, string parent) =>
        (s.Value ?? "").Contains($"<<{parent}:")
        || (s.Value ?? "").Contains($"<<{parent}>>:");
}