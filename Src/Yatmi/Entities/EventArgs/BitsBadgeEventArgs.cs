using System;

namespace Yatmi.Entities.EventArgs;

public class BitsBadgeEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who is sharing their bits badge
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Their message, if any
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// A system message auto created from the parameters (not sent from Twitch)
    /// </summary>
    public string AutoSystemMessage { get; }


    public BitsBadgeEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string message,
        string displayName,
        int bitsBadge
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
        Message = message;
        AutoSystemMessage = $"{displayName} just unlocked the {bitsBadge} badge!";
    }
}
