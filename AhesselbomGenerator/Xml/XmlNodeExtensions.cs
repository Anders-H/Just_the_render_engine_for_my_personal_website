using System;
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

    public static XmlNodeList GetItemsOrThrow(this XmlDocument me)
    {
        var rss = me.DocumentElement;

        if (rss == null)
            throw new Exception();

        if (rss.SelectNode("channel") is not XmlElement channel)
            throw new Exception();

        var items = channel.SelectNodes("item");

        return items ?? throw new Exception();
    }
}