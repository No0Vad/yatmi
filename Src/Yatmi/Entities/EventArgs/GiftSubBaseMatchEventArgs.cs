using System;

namespace Yatmi.Entities.EventArgs;

public class GiftSubBaseMatchEventArgs : BaseEventArgs
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
    /// Quantity of giftsubs matched
    /// </summary>
    public int Quantity { get; }


    public GiftSubBaseMatchEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string systemMessage,
        string advertiserName,
        int quantity
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
        Quantity = quantity;
    }
}