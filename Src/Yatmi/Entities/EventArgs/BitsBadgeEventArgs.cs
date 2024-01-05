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
    /// UserID of who is sharing their bits badge
    /// </summary>
    public string UserID { get; }

    /// <summary>
    /// Their message, if any
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// The bits badge unlocked
    /// </summary>
    public int BitsBadge { get; }

    /// <summary>
    /// A system message auto created from the parameters (not sent from Twitch)
    /// </summary>
    public string AutoSystemMessage { get; }


    public BitsBadgeEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
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
        UserID = userId;
        Message = message;
        BitsBadge = bitsBadge;
        AutoSystemMessage = $"{displayName} just unlocked the {bitsBadge} badge!";
    }
}
