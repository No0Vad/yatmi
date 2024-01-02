using System;
using Yatmi.Enum;

namespace Yatmi.Entities.EventArgs;

public class GiftSubscriptionEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who sent a gift subscription to someone
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// The UserID of who sent a gift subscription to someone
    /// </summary>
    public string UserID { get; }

    /// <summary>
    /// Username of who were gifted a subscription
    /// </summary>
    public string RecipientUsername { get; }

    /// <summary>
    /// The UserID of who were gifted a subscription
    /// </summary>
    public string RecipientUserID { get; }

    /// <summary>
    /// How many subscriptions they have gifted in total to the community
    /// </summary>
    public int TotalSubsGifted { get; }

    /// <summary>
    /// What tier of subscription they gifted
    /// </summary>
    public SubPlanTypes SubPlanType { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }

    /// <summary>
    /// How the gift was received
    /// </summary>
    public SubGiftTypes SubGiftType { get; }


    public GiftSubscriptionEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string recipientUsername,
        string recipientUserId,
        int totalSubsGifted,
        bool fromAnanonymousCommunityGift,
        string subPlan,
        string systemMessage
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
        UserID = userId;
        RecipientUsername = recipientUsername;
        RecipientUserID = recipientUserId;
        TotalSubsGifted = totalSubsGifted;
        SubPlanType = Helper.GetSubPlanType(subPlan);
        SystemMessage = systemMessage;

        SubGiftType = (username != KnownLogins.AN_ANONYMOUS_GIFTER && TotalSubsGifted == 0) || fromAnanonymousCommunityGift
            ? SubGiftTypes.CommunityGift
            : SubGiftTypes.PersonalGift;
    }
}
