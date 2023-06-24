namespace Yatmi.Entities.EventArgs;

public class RawIrcMessageEventArgs : TimestampEventArgs
{
    /// <summary>
    /// The raw IRC message
    /// </summary>
    public string RawIrcMessage { get; }

    /// <summary>
    /// Code, used to tag "where" it came from
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Details, used to describe "where" it came from
    /// </summary>
    public string Details { get; }


    public RawIrcMessageEventArgs(string rawIrcMessage, string code = null, string details = null)
    {
        RawIrcMessage = rawIrcMessage;
        Code = code;
        Details = details;
    }
}