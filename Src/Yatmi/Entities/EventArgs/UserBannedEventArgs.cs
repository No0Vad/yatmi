using System;

namespace Yatmi.Entities.EventArgs;

public class UserBannedEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who that was banned
    /// </summary>
    public string BannedUsername { get; }

    /// <summary>
    /// UserID of who that was banned
    /// </summary>
    public string BannedUserID { get; }


    public UserBannedEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string bannedUsername,
        string bannedUserId
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        BannedUsername = bannedUsername;
        BannedUserID = bannedUserId;
    }
}
