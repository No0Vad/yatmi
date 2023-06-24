using System;

namespace Yatmi.Entities.EventArgs;

public class ViewerMilestoneMessageEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who cancelled the raid
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }

    /// <summary>
    /// Type of milestone
    /// </summary>
    public decimal Category { get; }

    /// <summary>
    /// A value for the milestone
    /// </summary>
    public string Value { get; }


    public ViewerMilestoneMessageEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string systemMessage,
        string category,
        string value
    )
    : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
        SystemMessage = systemMessage;

        if (int.TryParse(category, out var intAmount))
        {
            Category = intAmount / 100m;
        }

        Value = value;
    }
}
