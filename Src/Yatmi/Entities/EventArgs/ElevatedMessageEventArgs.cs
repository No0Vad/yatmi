using System;

namespace Yatmi.Entities.EventArgs;

public class ElevatedMessageEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who sent the elevated message
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// The UserID of who sent the elevated message
    /// </summary>
    public string UserID { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }

    /// <summary>
    /// How much money that was used
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// In what currency it was paid
    /// </summary>
    public string Currency { get; }


    public ElevatedMessageEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string systemMessage,
        string amount,
        string currency
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

        if (int.TryParse(amount, out var intAmount))
        {
            Amount = intAmount / 100m;
        }

        Currency = currency;
    }
}
