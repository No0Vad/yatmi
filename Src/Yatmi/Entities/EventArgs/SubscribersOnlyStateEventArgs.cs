using System;

namespace Yatmi.Entities.EventArgs;

public class SubscribersOnlyStateEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// If true, subscribers only is active
    /// </summary>
    public bool IsActive { get; }


    public SubscribersOnlyStateEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        int subsOnly
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        IsActive = subsOnly == 1;
    }
}