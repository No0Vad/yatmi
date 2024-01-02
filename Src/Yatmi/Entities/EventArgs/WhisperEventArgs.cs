using System;

namespace Yatmi.Entities.EventArgs;

public class WhisperEventArgs : BaseEventArgs
{
    /// <summary>
    /// Username who send the bot a whisper message
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// UserID who send the bot a whisper message
    /// </summary>
    public string UserID { get; }

    /// <summary>
    /// The message that was sent
    /// </summary>
    public string Message { get; }


    public WhisperEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string username,
        string userId,
        string message
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Username = username;
        UserID = userId;
        Message = message;
    }
}
