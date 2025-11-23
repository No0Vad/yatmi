using System;

namespace Yatmi.Entities.EventArgs;

public class OneTapGiftRedeemedNoticeEventArgs : BaseEventArgs
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
    /// Amount of bits sepent
    /// </summary>
    public int BitsSpent { get; }

    /// <summary>
    /// The type of gift redeemed
    /// </summary>
    public string GiftId { get; }

    /// <summary>
    /// Username of who redeemed the gift
    /// </summary>
    public string UserDisplayName { get; }


    public OneTapGiftRedeemedNoticeEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string systemMessage,
        string sourceRoomId,
        int bitsSpent,
        string giftId,
        string userDisplayName
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
        BitsSpent = bitsSpent;
        GiftId = giftId;
        UserDisplayName = userDisplayName;
    }
}