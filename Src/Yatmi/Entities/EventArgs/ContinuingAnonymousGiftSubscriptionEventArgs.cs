using System;

namespace Yatmi.Entities.EventArgs;

public class ContinuingAnonymousGiftSubscriptionEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who that decided to continue a subscription they originally was gifted anonymously
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }


    public ContinuingAnonymousGiftSubscriptionEventArgs(
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
