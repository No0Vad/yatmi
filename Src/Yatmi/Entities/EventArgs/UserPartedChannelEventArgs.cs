using System;

namespace Yatmi.Entities.EventArgs;

public class UserPartedChannelEventArgs : BaseEventArgs
{
    /// <summary>
    /// Name of the channel the user just parted from
    /// </summary>
    public string PartedChannel { get; }

    /// <summary>
    /// Username of the user who just parted
    /// </summary>
    public string Username { get; }


    public UserPartedChannelEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string partedChannel,
        string username
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        PartedChannel = partedChannel;
        Username = username;
    }
}