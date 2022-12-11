using System;

namespace AhesselbomGenerator;

public class Link
{
    public string Url { get; }
    public string ClickableText { get; }
    public string ExtraText { get; }

    private Link(string? url, string? clickableText, string? extraText)
    {
        Url = (url ?? "").Trim();
        ClickableText = (clickableText ?? "").Trim();
        ExtraText = (extraText ?? "").Trim();
    }

    public static Link? Parse(string? raw)
    {
        raw = (raw ?? "").Trim();
        
        if (string.IsNullOrWhiteSpace(raw))
            return null;
        
        if (raw.IndexOf("|", StringComparison.Ordinal) < 0)
            return new Link(raw, "", "");
        
        var parts = raw.Split('|');
        
        return parts.Length > 2
            ? new Link(parts[0], parts[1], parts[2])
            : new Link(parts[0], parts[1], "");
    }

    public string GenerateLink()
    {
        if (string.IsNullOrWhiteSpace(Url))
            throw new Exception("Url");
        
        if (string.IsNullOrWhiteSpace(ClickableText) && !string.IsNullOrWhiteSpace(ExtraText))
            return $@"<a href=""{Url}"" target=""_blank"">{Url}</a> {ExtraText}";
        
        if (!string.IsNullOrWhiteSpace(ClickableText) && string.IsNullOrWhiteSpace(ExtraText))
            return $@"<a href=""{Url}"" target=""_blank"">{ClickableText}</a>";
        
        if (!string.IsNullOrWhiteSpace(ClickableText) && !string.IsNullOrWhiteSpace(ExtraText))
            return $@"<a href=""{Url}"" target=""_blank"">{ClickableText}</a> {ExtraText}";
        
        return $@"<a href=""{Url}"" target=""_blank"">{Url}</a>";
    }
}