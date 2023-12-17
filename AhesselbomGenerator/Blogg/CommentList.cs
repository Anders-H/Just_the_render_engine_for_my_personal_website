using System;
using System.Collections.Generic;

namespace AhesselbomGenerator.Blogg;

public class CommentList : List<Comment>
{
    public CommentList GetCommentsFromUrl(string url)
    {
        var result = new CommentList();

        foreach (var c in this)
        {
            var targetUrl = c.PageUrl;

            if (targetUrl.IndexOf('#') > -1)
                targetUrl = targetUrl.Split('#')[0];

            if (string.Compare(targetUrl, url, StringComparison.CurrentCultureIgnoreCase) == 0)
                result.Add(c);
        }

        return result;
    }
}