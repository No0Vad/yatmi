using System;

namespace Yatmi.Entities.EventArgs;

public class EmoteOnlyStateEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// True if emotes only is activated
    /// </summary>
    public bool IsActive { get; }


    public EmoteOnlyStateEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        int emoteOnly
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        IsActive = emoteOnly == 1;
    }
}