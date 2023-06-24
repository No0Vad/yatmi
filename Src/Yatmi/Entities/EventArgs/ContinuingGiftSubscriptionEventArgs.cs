using System;

namespace Yatmi.Entities.EventArgs;

public class ContinuingGiftSubscriptionEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who that decided to continue a subscription they originally was gifted
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Username who they originally got a gift subscription from
    /// </summary>
    public string GifterUsername { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }


    public ContinuingGiftSubscriptionEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string gifterUsername,
        string systemMessage
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
        GifterUsername = gifterUsername;
        SystemMessage = systemMessage;
    }
}
