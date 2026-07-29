using System;

namespace Yatmi.Entities.EventArgs;

public class ModiversaryEventArgs : BaseEventArgs
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
    /// Their message, if any
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }

    /// <summary>
    /// How many months they been a moderator
    /// </summary>
    public int Months { get; }


    public ModiversaryEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string message,
        string systemMessage,
        int months
    )
    : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
        UserID = userId;
        Message = message;
        SystemMessage = systemMessage;
        Months = months;
    }
}