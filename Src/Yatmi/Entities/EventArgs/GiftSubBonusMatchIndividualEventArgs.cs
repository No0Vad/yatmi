using System;
using Yatmi.Enum;

namespace Yatmi.Entities.EventArgs;

public class GiftSubBonusMatchIndividualEventArgs : BaseEventArgs
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
    /// Name of the advertiser
    /// </summary>
    public string AdvertiserName { get; }

    /// <summary>
    /// Username of who were gifted a subscription
    /// </summary>
    public string RecipientUsername { get; }

    /// <summary>
    /// The UserID of who were gifted a subscription
    /// </summary>
    public string RecipientUserID { get; }

    /// <summary>
    /// What tier of subscription they gifted
    /// </summary>
    public SubPlanTypes SubPlanType { get; }


    public GiftSubBonusMatchIndividualEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string systemMessage,
        string advertiserName,
        string recipientUsername,
        string recipientUserId,
        string subPlan
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
        AdvertiserName = advertiserName;

        RecipientUsername = recipientUsername;
        RecipientUserID = recipientUserId;
        SubPlanType = Helper.GetSubPlanType(subPlan);
    }
}