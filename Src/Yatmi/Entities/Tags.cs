using System;
using System.Collections.Generic;

namespace Yatmi.Entities;

public class Tags : Dictionary<string, string>
{
    /// <summary>
    /// Try find a tag by its key, and return it as a string
    /// </summary>
    /// <param name="key">Key of the tag to look for</param>
    /// <param name="defaultValue">Default value if tag is not found</param>
    public string GetStringValue(string key, string defaultValue = default)
    {
        if (TryGetValue(key, out var strValue))
        {
            return strValue;
        }

        return defaultValue;
    }


    /// <summary>
    /// Try find a tag by its key, and return it as a int
    /// </summary>
    /// <param name="key">Key of the tag to look for</param>
    /// <param name="defaultValue">Default value if tag is not found</param>
    public int GetIntValue(string key, int defaultValue = default)
    {
        if (TryGetValue(key, out var strValue) && int.TryParse(strValue, out var intValue))
        {
            return intValue;
        }

        return defaultValue;
    }


    /// <summary>
    /// Parse the raw tags and creates a new <see cref="Tags"/>
    /// </summary>
    /// <param name="rawTags">The raw tags</param>
    public static Tags Parse(string rawTags)
    {
        var tagsEntity = new Tags();

        if (string.IsNullOrEmpty(rawTags))
        {
            return tagsEntity;
        }

        // Lazy fix at the moment
        var span = (rawTags + ";").AsSpan()[1..];
        int index;

        while ((index = span.IndexOf(';')) != -1)
        {
            var data = span[..index];

            var split = data.IndexOf('=');
            var key = data[..split];
            var val = data[(split + 1)..];

            span = span[(index + 1)..];

            tagsEntity.Add(key.ToString(), val.ToString().Replace("\\s", " "));
        }

        return tagsEntity;
    }
}
