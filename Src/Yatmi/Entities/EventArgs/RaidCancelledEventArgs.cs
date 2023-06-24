using System;

namespace Yatmi.Entities.EventArgs;

public class RaidCancelledEventArgs : BaseEventArgs
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


    public RaidCancelledEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string systemMessage
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
        SystemMessage = systemMessage;
    }
}
