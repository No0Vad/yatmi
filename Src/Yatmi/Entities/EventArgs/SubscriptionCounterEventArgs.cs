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
        int subscriptionCount,
        SubPlanTypes subPlanType
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
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
            1,
            SubPlanTypes.Unknown
        );
    }
}
