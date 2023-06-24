using System;

namespace Yatmi.Entities.EventArgs;

public class NoticeEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The notice message from Twitch
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// What type of notice that was sent
    /// </summary>
    public string NoticeType { get; }


    public NoticeEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string message,
        string msgId
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Message = message;
        NoticeType = msgId;
    }
}