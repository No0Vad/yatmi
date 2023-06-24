using System;

namespace Yatmi.Entities.EventArgs;

public class RoomStateEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The state of the emote only mode
    /// </summary>
    public bool EmoteOnly { get; }

    /// <summary>
    /// The state of the followers only mode
    /// </summary>
    public bool FollowersOnly { get; }

    /// <summary>
    /// How long they must have been a follower
    /// </summary>
    public TimeSpan FollowersTime { get; }

    /// <summary>
    /// How long to wait before a message can be sent
    /// </summary>
    public TimeSpan SlowMode { get; }

    /// <summary>
    /// State of the subscribers only mode
    /// </summary>
    public bool SubsOnly { get; }


    public RoomStateEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        int emoteOnly,
        int followersOnly,
        int slowMode,
        int subsOnly
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        EmoteOnly = emoteOnly == 1;

        if (followersOnly == -1)
        {
            FollowersOnly = false;
            FollowersTime = default;
        }
        else
        {
            FollowersOnly = true;
            FollowersTime = TimeSpan.FromMinutes(followersOnly);
        }

        SlowMode = TimeSpan.FromSeconds(slowMode);
        SubsOnly = subsOnly == 1;
    }
}