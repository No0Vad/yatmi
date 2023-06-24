namespace Yatmi.Entities.EventArgs;

public class ParsedIrcMessageEventArgs : TimestampEventArgs
{
    /// <summary>
    /// The result of a parsed raw IRC message
    /// </summary>
    public ParsedIrcMessage ParsedIrcMessage { get; }


    public ParsedIrcMessageEventArgs(ParsedIrcMessage parsedIrcMessage)
    {
        ParsedIrcMessage = parsedIrcMessage;
    }
}
