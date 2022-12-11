using System.Xml;

namespace AhesselbomGenerator.Xml;

public static class XmlNodeExtensions
{
    public static XmlNode? SelectNode(this XmlNode me, string name) =>
        me.SelectSingleNode(name);

    public static string GetText(this XmlNode me, string name) =>
        me.SelectSingleNode(name)?.InnerText ?? "";

    public static string GetAttributeValue(this XmlNode? me, string name) =>
        me?.Attributes?.GetNamedItem(name)?.Value ?? "";
}