using System;

namespace Yatmi.Entities.EventArgs;

public class OneTapStreakExpiredNoticeEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of the shared notice
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// UserID of the shared notice
    /// </summary>
    public string UserID { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }

    /// <summary>
    /// The room ID this message was sent from, it was not the room the bot is joined to.
    /// </summary>
    public string SourceRoomId { get; }

    /// <summary>
    /// The type of gift
    /// </summary>
    public string GiftId { get; }

    /// <summary>
    /// Name of the biggest (or first) contributor
    /// </summary>
    public string Contributor1 { get; }

    /// <summary>
    /// How many taps from the biggest (or first) contributor
    /// </summary>
    public int Contributor1Taps { get; }

    /// <summary>
    /// Name of the second biggest (or second) contributor
    /// </summary>
    public string Contributor2 { get; }

    /// <summary>
    /// How many taps from the second biggest (or second) contributor
    /// </summary>
    public int Contributor2Taps { get; }

    /// <summary>
    /// How many taps from the largest contributor
    /// </summary>
    public int LargestContributorCount { get; }

    /// <summary>
    /// Streak size in bits
    /// </summary>
    public int StreakSizeBits { get; }

    /// <summary>
    /// Streak size in taps
    /// </summary>
    public int StreakSizeTaps { get; }


    public OneTapStreakExpiredNoticeEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string systemMessage,
        string sourceRoomId,
        string giftId,
        string contributor1,
        int contributor1Taps,
        string contributor2,
        int contributor2Taps,
        int largestContributorCount,
        int streakSizeBits,
        int streakSizeTaps
    )
    : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
        UserID = userId;
        SystemMessage = systemMessage;
        SourceRoomId = sourceRoomId;
        GiftId = giftId;
        Contributor1 = contributor1;
        Contributor1Taps = contributor1Taps;
        Contributor2 = contributor2;
        Contributor2Taps = contributor2Taps;
        LargestContributorCount = largestContributorCount;
        StreakSizeBits = streakSizeBits;
        StreakSizeTaps = streakSizeTaps;
    }
}