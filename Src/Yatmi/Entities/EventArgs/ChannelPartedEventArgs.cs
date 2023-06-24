using System;

namespace Yatmi.Entities.EventArgs;

public class ChannelPartedEventArgs : BaseEventArgs
{
    /// <summary>
    /// Name of the channel the bot just parted from
    /// </summary>
    public string PartedChannel { get; }


    public ChannelPartedEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string partedChannel
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        PartedChannel = partedChannel;
    }
}
