using System;

namespace Yatmi.Entities.EventArgs;

public class SharedChatNoticeEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of the shared notice
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// UserID of the shared notice
    /// </summary>
    public string UserID { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }

    /// <summary>
    /// The room ID this message was sent from, it was not the room the bot is joined to.
    /// </summary>
    public string SourceRoomId { get; }


    public SharedChatNoticeEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string systemMessage,
        string sourceRoomId
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
        SourceRoomId = sourceRoomId;
    }
}