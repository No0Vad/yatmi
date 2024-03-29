﻿using System;

namespace Yatmi.Entities.EventArgs;

public class SubscriptionPayForwardEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who that decided to pay forward their subscription they were gifted, but gifting a subscription to someone else
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// UserID of who that decided to pay forward their subscription they were gifted, but gifting a subscription to someone else
    /// </summary>
    public string UserID { get; }

    /// <summary>
    /// Username who they originally got a gift subscription from
    /// </summary>
    public string GifterUsername { get; }

    /// <summary>
    /// UserID who they originally got a gift subscription from
    /// </summary>
    public string GifterUserID { get; }

    /// <summary>
    /// Username who they decided to gift a subscription to
    /// </summary>
    public string ReceiverUsername { get; }

    /// <summary>
    /// UserID who they decided to gift a subscription to
    /// </summary>
    public string ReceiverUserID { get; }

    /// <summary>
    /// The system message text from Twitch
    /// </summary>
    public string SystemMessage { get; }


    public SubscriptionPayForwardEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string username,
        string userId,
        string gifterUsername,
        string gifterUserId,
        string receiverUsername,
        string receiverUserId,
        string systemMessage
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Username = username;
        UserID = userId;
        GifterUsername = gifterUsername;
        GifterUserID = gifterUserId;
        ReceiverUsername = receiverUsername;
        ReceiverUserID = receiverUserId;
        SystemMessage = systemMessage;
    }
}
