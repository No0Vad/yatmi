using System;

namespace Yatmi.Entities.EventArgs;

public class BaseEventArgs : TimestampEventArgs
{
    /// <summary>
    /// If <see cref="TwitchChatClient.IncludeParsedIrcMessagesInEvents"/> is true, the parsed message is stored here. If not, this is null
    /// </summary>
    public ParsedIrcMessage ParsedIrcMessage { get; }

    public BaseEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp
    ) : base(timestamp)
    {
        ParsedIrcMessage = parsedIrcMessage;
    }
}
