using System;
using Yatmi.Enum;

namespace Yatmi.Entities.EventArgs;

public class SubscriptionCounterEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of the subscription
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// UserID of the subscription
    /// </summary>
    public string UserID { get; }

    /// <summary>
    /// How many subscriptions there were
    /// </summary>
    public int SubscriptionCount { get; set; }

    /// <summary>
    /// What tier the subscriptions was
    /// </summary>
    public SubPlanTypes SubPlanType { get; }


    public SubscriptionCounterEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        int subscriptionCount,
        SubPlanTypes subPlanType
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
        UserID = userId;
        SubscriptionCount = subscriptionCount;
        SubPlanType = subPlanType;
    }


    public static SubscriptionCounterEventArgs From(SubscribeEventArgs e)
    {
        return new SubscriptionCounterEventArgs(
            e.ParsedIrcMessage,
            e.Timestamp,
            e.Channel,
            e.Username,
            e.UserID,
            1,
            e.SubPlanType
        );
    }


    public static SubscriptionCounterEventArgs From(ResubscribeEventArgs e)
    {
        return new SubscriptionCounterEventArgs(
            e.ParsedIrcMessage,
            e.Timestamp,
            e.Channel,
            e.Username,
            e.UserID,
            1,
            e.SubPlanType
        );
    }


    public static SubscriptionCounterEventArgs From(GiftSubscriptionEventArgs e)
    {
        return new SubscriptionCounterEventArgs(
            e.ParsedIrcMessage,
            e.Timestamp,
            e.Channel,
            e.Username,
            e.UserID,
            1,
            e.SubPlanType
        );
    }


    public static SubscriptionCounterEventArgs From(CommunityGiftSubscriptionEventArgs e)
    {
        return new SubscriptionCounterEventArgs(
            e.ParsedIrcMessage,
            e.Timestamp,
            e.Channel,
            e.Username,
            e.UserID,
            e.SubscriptionsGifted,
            e.SubPlanType
        );
    }


    public static SubscriptionCounterEventArgs From(ContinuingGiftSubscriptionEventArgs e)
    {
        return new SubscriptionCounterEventArgs(
            e.ParsedIrcMessage,
            e.Timestamp,
            e.Channel,
            e.Username,
            e.UserID,
            1,
            SubPlanTypes.Unknown
        );
    }
}
