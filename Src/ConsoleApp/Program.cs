using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yatmi.Entities.EventArgs;

namespace Yatmi.Example;

internal static class Program
{
    internal static TwitchChatClient _client;
    internal static readonly object _lock = new();

    static async Task Main()
    {
        await using var client = new TwitchChatClient();
        _client = client;

        client.OnPing += Client_OnPing;
        client.OnLog += Client_OnLog;

        client.OnRawIrcMessage += Client_OnRawIrcMessage;
        client.OnUnknownMessage += Client_OnUnknownMessage;
        client.OnParsedIrcMessage += Client_OnParsedIrcMessage;
        client.OnAuthFailed += Client_OnAuthFailed;
        client.OnNotice += Client_OnNotice;

        client.OnConnected += Client_OnConnected;
        client.OnDisconnected += Client_OnDisconnected;

        client.OnChannelJoined += Client_OnChannelJoined;
        client.OnChannelParted += Client_OnChannelParted;
        //client.OnUserJoinedChannel += Client_OnUserJoinedChannel;
        //client.OnUserPartedChannel += Client_OnUserPartedChannel;
        client.OnChatMessage += Client_OnChatMessage;
        client.OnWhisperMessage += Client_OnWhisperMessage;
        client.OnBitsChatMessage += Client_OnBitsChatMessage;
        client.OnRaided += Client_OnRaided;
        client.OnRaidCancelled += Client_OnRaidCancelled;
        client.OnHosted += Client_OnHosted;

        client.OnSubscribe += Client_OnSubscribe;
        client.OnResubscribe += Client_OnResubscribe;
        client.OnGiftSubscription += Client_OnGiftSubscribe;
        client.OnCommunityGiftSubscription += Client_OnCommunityGiftSubscribe;
        client.OnCommunityPayForward += Client_OnCommunityPayForward;
        client.OnSubscriptionPayForward += Client_OnSubscriptionPayForward;
        client.OnPrimeUpgradeSubscription += Client_OnPrimeUpgradeSubscription;
        client.OnSubscriptionCounter += Client_OnSubscriptionCounter;
        client.OnBitsBadge += Client_OnBitsBadge;
        client.OnElevatedMessage += Client_OnElevatedMessage;
        client.OnViewerMilestoneMessage += Client_OnViewerMilestoneMessage;
        client.OnContinuingGiftSubscription += Client_OnContinuingGiftSubscription;
        client.OnContinuingAnonymousGiftSubscription += Client_OnContinuingAnonymousGiftSubscription;

        client.OnUserTimeout += Client_OnUserTimeout;
        client.OnUserBanned += Client_OnUserBanned;
        client.OnChatCleared += Client_OnChatCleared;
        client.OnChatMessageDeleted += Client_OnChatMessageDeleted;

        //client.OnNamesList += Client_OnNamesList;
        client.OnRoomState += Client_OnRoomState;
        client.OnEmoteOnlyState += Client_OnEmoteOnlyState;
        client.OnFollowersOnlyState += Client_OnFollowersOnlyState;
        client.OnSlowModeState += Client_OnSlowModeState;
        client.OnSubscribersOnlyState += Client_OnSubsOnlyState;

        if (true)
        {
            UseSimulatedMessages();
        }

        //await client.ConnectAsync();
        //await client.JoinChannelAsync(Env.Channel.Value);

        //_ = Task.Delay(10_000).ContinueWith(async _ =>
        //{
        //    Console.WriteLine("Force disconnect for testing!");
        //    await _client.DisconnectTestAsync();
        //});

        ColorWriteLine("Press any key to exit...", ConsoleColor.Cyan);
        Console.ReadKey();

        ColorWriteLine("Shutdown!", ConsoleColor.Yellow);
    }

    internal static void Client_OnPing(object sender, TimestampEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}] Responded to a ping!");
    }


    internal static void Client_OnLog(object sender, LogEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}] <LOG> {e.Message}", ConsoleColor.DarkGray);
    }


    internal static void Client_OnRawIrcMessage(object sender, RawIrcMessageEventArgs e)
    {
        // Save raw IRC message for analyze 
        if (e.RawIrcMessage.Contains(" PRIVMSG "))
        {
            return;
        }

        if (!Env.DataFolder.Exists || !Directory.Exists(Env.DataFolder.Value))
        {
            ColorWriteLine($"[OnRawIrcMessage] Make sure the environment variable {Env.DataFolder.Name} exists with a valid path!", ConsoleColor.Yellow);
            return;
        }

        File.AppendAllText(Path.Combine(Env.DataFolder.Value, "RawDump.txt"), e.RawIrcMessage + Environment.NewLine);
    }

    internal static void Client_OnUnknownMessage(object sender, RawIrcMessageEventArgs e)
    {
        ColorWriteLine($"[Unknown][{e.Code}] {e.RawIrcMessage} | {e.Details}", ConsoleColor.DarkYellow);
    }

    internal static void Client_OnParsedIrcMessage(object sender, ParsedIrcMessageEventArgs e)
    {
        // Message was parsed
    }

    internal static void Client_OnAuthFailed(object sender, TimestampEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}] Incorrect login!", ConsoleColor.Red);
    }

    internal static void Client_OnNotice(object sender, NoticeEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}] NOTICE: {e.Message} -- ({e.NoticeType})", ConsoleColor.Yellow);
    }

    internal static void Client_OnConnected(object sender, TimestampEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}] Connected to Twitch IRC!", ConsoleColor.Green);
    }

    internal static void Client_OnDisconnected(object sender, TimestampEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}] Disconnected from Twitch IRC!", ConsoleColor.Red);
    }

    internal static void Client_OnChannelJoined(object sender, ChannelJoinedEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}] Joined channel '{e.JoinedChannel}'", ConsoleColor.Green);
    }

    internal static void Client_OnChannelParted(object sender, ChannelPartedEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}] Parted channel '{e.PartedChannel}'", ConsoleColor.Red);
    }

    internal static void Client_OnUserJoinedChannel(object sender, UserJoinedChannelEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.JoinedChannel}] <<< {e.Username} just joined!", ConsoleColor.Green);
        Console.ResetColor();
    }

    internal static void Client_OnUserPartedChannel(object sender, UserPartedChannelEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.PartedChannel}] >>> {e.Username} just left!", ConsoleColor.Red);
    }

    internal static void Client_OnChatMessage(object sender, ChatMessageEventArgs e)
    {
        ColorWriteLine(string.Join(" ", new[]
        {
            $"[{e.Timestamp}][{e.Channel}]",
            e.IsStaff ? "[Staff]" : "",
            e.IsBroadcaster ? "[Stream]" : "",
            e.IsFounder ? "[Founder]" : e.IsSubscriber ? "[Sub]" : "",
            e.IsModerator ? "[Mod]" : e.IsVip ? "[Vip]" : "",
            e.PaidChat != null ? $"[Paid {e.PaidChat.Amount} {e.PaidChat.Currency}]" : "",
            $"| {e.Username} (ID:{e.UserID}) wrote:",
            "["+e.MessageType + "]",
            e.IsMe ? "/me" : "",
            e.Message
        }.Where(a => a.Length > 0)));

        if (!_client.IsAnonymous && e.Message.StartsWith("!hello there"))
        {
            _ = _client.SendMessageAsync(e.Channel, $"General {e.Username.ToUpper()}!", e.ID);
        }
        else if (!_client.IsAnonymous && e.Message.StartsWith("!timeout"))
        {
            _ = _client.SendMessageAsync(e.Channel, $"/timeout {e.Username} 5 Asked for it");
        }
        else if (!_client.IsAnonymous && e.Message.StartsWith("!ban"))
        {
            _ = _client.SendMessageAsync(e.Channel, $"/ban {e.Username} Asked for it");
        }
        else if (!_client.IsAnonymous && e.Message.StartsWith("!delete"))
        {
            _ = _client.SendMessageAsync(e.Channel, $"/delete {e.ID}");
        }
        else if (e.Message.StartsWith("!disconnect"))
        {
            _ = _client.DisconnectAsync(false);
        }
    }

    internal static void Client_OnWhisperMessage(object sender, WhisperEventArgs e)
    {
        // https://discuss.dev.twitch.tv/t/my-bot-cant-send-a-whisp/21481
        ColorWriteLine($"[{e.Timestamp}] <WHISPER> '{e.Username} (ID:{e.UserID})': {e.Message}", ConsoleColor.Green);
    }

    internal static void Client_OnBitsChatMessage(object sender, ChatMessageEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] '{e.Bits}' from {e.Username}!", ConsoleColor.Magenta);
    }

    internal static void Client_OnRaided(object sender, RaidEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] '{e.Raider}' (ID:{e.RaiderID}) has raided with {e.Viewers} viewers!", ConsoleColor.Magenta);
    }

    internal static void Client_OnRaidCancelled(object sender, RaidCancelledEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.SystemMessage}", ConsoleColor.Magenta);
    }

    internal static void Client_OnHosted(object sender, HostEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] '{e.Hosted}' is now hosted with {e.Viewers} viewers!", ConsoleColor.Magenta);
    }

    internal static void Client_OnSubscribe(object sender, SubscribeEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.SystemMessage} | {e.Username} (ID:{e.UserID}) | {e.Months} | {e.SubPlanType}", ConsoleColor.Magenta);
    }

    internal static void Client_OnResubscribe(object sender, ResubscribeEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.SystemMessage} | {e.Username} (ID:{e.UserID}) | {e.Months} | {e.SubPlanType}", ConsoleColor.Magenta);
    }

    internal static void Client_OnGiftSubscribe(object sender, GiftSubscriptionEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.SystemMessage} | {e.Username} (ID:{e.UserID}) | {e.RecipientUsername} (ID:{e.RecipientUserID}) | {e.SubGiftType} | {e.SubPlanType} | {e.TotalSubsGifted} (total)", ConsoleColor.Magenta);
    }

    internal static void Client_OnCommunityGiftSubscribe(object sender, CommunityGiftSubscriptionEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.SystemMessage} | {e.Username} (ID:{e.UserID}) | {e.SubscriptionsGifted} | {e.SubPlanType} | {e.TotalSubscriptionsGifted} (total)", ConsoleColor.Magenta);
    }

    internal static void Client_OnCommunityPayForward(object sender, CommunityPayForwardEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.SystemMessage} | {e.Username} (ID:{e.UserID}) | {e.GifterUsername} (ID:{e.GifterUserID})", ConsoleColor.Magenta);
    }

    internal static void Client_OnSubscriptionPayForward(object sender, SubscriptionPayForwardEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.SystemMessage} | {e.Username} (ID:{e.UserID}) | {e.GifterUsername} (ID:{e.GifterUserID}) | {e.ReceiverUsername}", ConsoleColor.Magenta);
    }

    internal static void Client_OnPrimeUpgradeSubscription(object sender, PrimeUpgradeSubscriptionEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.SystemMessage} | {e.Username} (ID:{e.UserID}) | {e.SubPlanType}", ConsoleColor.Magenta);
    }

    internal static void Client_OnSubscriptionCounter(object sender, SubscriptionCounterEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.Username} (ID:{e.UserID}) | {e.SubscriptionCount} | {e.SubPlanType} -- COUNTER", ConsoleColor.Cyan);
    }

    internal static void Client_OnBitsBadge(object sender, BitsBadgeEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.AutoSystemMessage}", ConsoleColor.Magenta);
    }

    internal static void Client_OnElevatedMessage(object sender, ElevatedMessageEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] [Elevated] {e.SystemMessage}", ConsoleColor.Magenta);
    }

    internal static void Client_OnViewerMilestoneMessage(object sender, ViewerMilestoneMessageEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] [Milestone] {e.SystemMessage}", ConsoleColor.Magenta);
    }

    internal static void Client_OnContinuingGiftSubscription(object sender, ContinuingGiftSubscriptionEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.SystemMessage} | {e.Username} (ID:{e.UserID}) | {e.GifterUsername}", ConsoleColor.Magenta);
    }

    internal static void Client_OnContinuingAnonymousGiftSubscription(object sender, ContinuingAnonymousGiftSubscriptionEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.SystemMessage} | {e.Username} (ID:{e.UserID})", ConsoleColor.Magenta);
    }

    internal static void Client_OnUserTimeout(object sender, UserTimeoutEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.TimedoutUsername} (ID:{e.TimedoutUserID}) was timeout! {e.Duration}", ConsoleColor.Yellow);
    }

    internal static void Client_OnUserBanned(object sender, UserBannedEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] {e.BannedUsername} (ID:{e.BannedUserID}) was banned!", ConsoleColor.Red);
    }

    internal static void Client_OnChatCleared(object sender, ChatClearedEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] Chat was cleared!", ConsoleColor.White);
    }

    internal static void Client_OnChatMessageDeleted(object sender, ChatMessageDeletedEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] Message from {e.Username} (ID:{e.UserID}) was deleted! ** {e.DeletedMessage} **", ConsoleColor.White);
    }

    internal static void Client_OnNamesList(object sender, NamesListEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] Usernames in chat >> {string.Join(", ", e.Usernames)}", ConsoleColor.Cyan);
    }

    internal static void Client_OnRoomState(object sender, RoomStateEventArgs e)
    {
        ColorWriteLines(new[]
        {
            $"[{e.Timestamp}][{e.Channel}] EmotesOnly = {e.EmoteOnly} | FollowersOnly = {e.FollowersOnly}, {e.FollowersTime}",
            $"[{e.Timestamp}][{e.Channel}] SlowMode = {e.SlowMode} | SubsOnly = {e.SubsOnly}",
        }, ConsoleColor.Cyan);
    }

    internal static void Client_OnEmoteOnlyState(object sender, EmoteOnlyStateEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] [Emotes] EmotesOnly = {e.IsActive}", ConsoleColor.Cyan);
    }

    internal static void Client_OnFollowersOnlyState(object sender, FollowersOnlyStateEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] [Followers] FollowersOnly = {e.FollowersOnly},{e.FollowersTime}", ConsoleColor.Cyan);
    }

    internal static void Client_OnSlowModeState(object sender, SlowModeStateEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] [SlowMode] SlowMode = {e.SlowMode}", ConsoleColor.Cyan);
    }

    internal static void Client_OnSubsOnlyState(object sender, SubscribersOnlyStateEventArgs e)
    {
        ColorWriteLine($"[{e.Timestamp}][{e.Channel}] [SubsOnly] SubsOnly = {e.IsActive}", ConsoleColor.Cyan);
    }



    private static void UseSimulatedMessages()
    {
        static string lineCleaner(string s)
        {
            s = s.Trim();

            if (s.StartsWith("@\""))
            {
                s = s.TrimStart('@');
            }

            return s.TrimEnd(',').Trim('"');
        }

        if (!Env.DataFolder.Exists || !Directory.Exists(Env.DataFolder.Value))
        {
            throw new Exception($"Make sure the environment variable {Env.DataFolder.Name} exists with a valid path!");
        }

        var fileSystemWatcher = new FileSystemWatcher(Env.DataFolder.Value, "Consumer.txt");

        fileSystemWatcher.Changed += async (s, e) =>
        {
            // File was changed!
            fileSystemWatcher.EnableRaisingEvents = false;
            await Task.Delay(100);

            // Read all the lines and simulate them for quicker testing
            var rawIrcs = await File.ReadAllLinesAsync(e.FullPath);
            var arrRawIrcs = rawIrcs
                .Select(a => lineCleaner(a))
                .Where(a => !string.IsNullOrEmpty(a) && !a.StartsWith("//"))
                .ToArray();

            Console.WriteLine("");
            await _client.SimulateMessagesAsync(arrRawIrcs);

            fileSystemWatcher.EnableRaisingEvents = true;
        };

        fileSystemWatcher.EnableRaisingEvents = true;
    }

    private static void ColorWriteLine(string message, ConsoleColor? color = null)
    {
        if (color == null)
        {
            Console.WriteLine(message);
            return;
        }

        lock (_lock)
        {
            Console.ForegroundColor = color.Value;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    private static void ColorWriteLines(IEnumerable<string> messages, ConsoleColor? color = null)
    {
        if (color == null)
        {
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }

            return;
        }

        lock (_lock)
        {
            Console.ForegroundColor = color.Value;

            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }

            Console.ResetColor();
        }
    }
}