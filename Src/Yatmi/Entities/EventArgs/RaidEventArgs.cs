﻿using System;

namespace Yatmi.Entities.EventArgs;

public class RaidEventArgs : BaseEventArgs
{
    /// <summary>
    /// In which channel this event occurred
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Username of who is raiding the channel
    /// </summary>
    public string Raider { get; }

    /// <summary>
    /// UserID of who is raiding the channel
    /// </summary>
    public string RaiderID { get; }

    /// <summary>
    /// How many viewers they brought
    /// </summary>
    public int Viewers { get; }


    public RaidEventArgs(
        ParsedIrcMessage parsedIrcMessage,
        DateTime timestamp,
        string channel,
        string raider,
        string raiderId,
        int viewers
    ) : base(
        parsedIrcMessage,
        timestamp
    )
    {
        Channel = channel;
        Raider = raider;
        RaiderID = raiderId;
        Viewers = viewers;
    }
}
