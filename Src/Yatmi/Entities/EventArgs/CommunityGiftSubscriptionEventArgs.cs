using System;
using Yatmi.Enum;

namespace Yatmi.Entities.EventArgs;

public class CommunityGiftSubscriptionEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who sent the gift subscriptions to the community
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// The UserID of the user
    /// </summary>
    public string UserID { get; }

    /// <summary>
    /// How many subscription they gifted to the community
    /// </summary>
    public int SubscriptionsGifted { get; }

    /// <summary>
    /// How many subscriptions they have gifted in total to the community
    /// </summary>
    public int TotalSubscriptionsGifted { get; }

    /// <summary>
    /// What tier of subscription they gifted
    /// </summary>
    public SubPlanTypes SubPlanType { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }


    public CommunityGiftSubscriptionEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        int subscriptionsGifted,
        int totalSubscriptionsGifted,
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
        SubscriptionsGifted = subscriptionsGifted;
        TotalSubscriptionsGifted = totalSubscriptionsGifted;
        SubPlanType = Helper.GetSubPlanType(subPlan);
        SystemMessage = systemMessage;
    }
}
