using System;

namespace Yatmi.Entities.EventArgs;

public class ViewerMilestoneMessageEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who shared the milestone
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// UserID of who shared the milestone
    /// </summary>
    public string UserID { get; }

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
        string userId,
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
        UserID = userId;
        SystemMessage = systemMessage;

        if (int.TryParse(category, out var intAmount))
        {
            Category = intAmount / 100m;
        }

        Value = value;
    }
}
