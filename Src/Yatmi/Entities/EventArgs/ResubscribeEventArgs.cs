using System;
using Yatmi.Enum;

namespace Yatmi.Entities.EventArgs;

public class ResubscribeEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who that just resubscribed
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// UsernID of who that just resubscribed
    /// </summary>
    public string UserID { get; }

    /// <summary>
    /// The resubscribe message, if any
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// If <see cref="TwitchChatClient.ParseEmotesInMessages"/> is true, the emotes data is stored here. If not, this is null
    /// </summary>
    public Emotes Emotes { get; }

    /// <summary>
    /// How many months they have been subscribed
    /// </summary>
    public int Months { get; }

    /// <summary>
    /// What tier of their subscription is
    /// </summary>
    public SubPlanTypes SubPlanType { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }


    public ResubscribeEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string message,
        string emotes,
        int months,
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
        Message = message;
        Emotes = Emotes.Parse(emotes);
        Months = months;
        SubPlanType = Helper.GetSubPlanType(subPlan);
        SystemMessage = systemMessage;
    }
}