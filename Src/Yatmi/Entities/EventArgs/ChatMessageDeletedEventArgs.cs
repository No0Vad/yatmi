using System;

namespace Yatmi.Entities.EventArgs;

public class ChatMessageDeletedEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of the message that got deleted
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// The deleted message
    /// </summary>
    public string DeletedMessage { get; }

    /// <summary>
    /// MessageID of the message that got deleted
    /// </summary>
    public string DeletedMessageID { get; }


    public ChatMessageDeletedEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string deletedMessage,
        string deletedMessageId
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
        DeletedMessage = deletedMessage;
        DeletedMessageID = deletedMessageId;
    }
}
