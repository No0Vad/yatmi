using System;

namespace Yatmi.Entities.EventArgs;

public class OneTapBreakpointAchievedNoticeEventArgs : BaseEventArgs
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
    /// How bigh the milestone was
    /// </summary>
    public long BreakpointNumber { get; }

    /// <summary>
    /// Bits needed for the achived breakpoint
    /// </summary>
    public long BreakpointThresholdBits { get; }


    public OneTapBreakpointAchievedNoticeEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string systemMessage,
        string sourceRoomId,
        string giftId,
        int breakpointNumber,
        int breakpointThresholdBits
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
        BreakpointNumber = breakpointNumber;
        BreakpointThresholdBits = breakpointThresholdBits;
    }
}