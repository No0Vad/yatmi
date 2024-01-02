using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Yatmi.Core;
using Yatmi.Entities;
using Yatmi.Entities.EventArgs;
using Yatmi.Enum;
using Timer = System.Threading.Timer;

namespace Yatmi;

public sealed partial class TwitchChatClient : IAsyncDisposable
{
    private const string HOST = "irc-ws.chat.twitch.tv";

    private readonly List<string> _joinedChannels;
    private readonly string _oauth;
    private readonly bool _useSsl;
    private readonly Cooldown _reconnectCooldown;
    private readonly Refresher _shouldConnectRefresher;

    private bool _isDisposing;
    private bool _shouldBeConnected;
    private bool _canFireDisconnect;
    private bool _reconnectPending;
    private DateTime? _tryReconnectAfter;

    private ClientWebSocket _client;
    private CancellationTokenSource _tokenSource;

    private readonly WaitableConcurrentQueue<string> _joinQueue;
    private readonly WaitableConcurrentQueue<string> _messageQueue;
    private readonly Timer _monitorTimer;

    private readonly LimitedCollection<string> _anonymousCommunityGiftIds;


    /// <summary>
    /// The username of the connected bot
    /// </summary>
    public string Username { get; }


    /// <summary>
    /// Indicates if the bot is anonymous or known with a username + OAuth token
    /// </summary>
    public bool IsAnonymous { get; }


    /// <summary>
    /// Indicates the instance is connected to the Twitch Chat Network
    /// </summary>
    public bool IsConnected => _client?.State == WebSocketState.Open;


    /// <summary>
    /// The list of channels currently joined
    /// </summary>
    public IReadOnlyList<string> ConnectedChannels => _joinedChannels;


    /// <summary>
    /// If true, the <see cref="ParsedIrcMessage"/> entity will be passed to the events. Default is false
    /// </summary>
    public bool IncludeParsedIrcMessagesInEvents { get; set; }


    /// <summary>
    /// If true, the raw IRC message will be kept in the <see cref="ParsedIrcMessage"/> entity. Default is false
    /// </summary>
    public bool KeepRawIrcMessageAfterParsed { get; set; }


    /// <summary>
    /// If true, auto reconnect attempts are made if connection is lost. Default is true
    /// </summary>
    public bool AutoReconnect { get; set; } = true;


    /// <summary>
    /// If true, the joined channels will be remembered during a reconnect. Else you have to join them again manually. Default is true
    /// </summary>
    public bool RememberChannelsOnReconnect { get; set; } = true;


    /// <summary>
    /// If true, the emote data will be parsed and passed to the messages. Default is false
    /// </summary>
    public bool ParseEmotesInMessages { get; set; }

    /// <summary>
    /// Used in the monitoring logic to decide when to log a status, if 0 no logging is made.
    /// <para>
    /// How it is used:
    /// <c>DateTime.Now.Minute % MonitorLogModular == 0</c>
    /// </para>
    /// </summary>
    public int MonitorLogModular { get; set; }


    #region Events

    /// <summary>
    /// Fired when a log entry is made.
    /// </summary>
    public event EventHandler<LogEventArgs> OnLog;

    /// <summary>
    /// Fired when a PING command has been responded to
    /// </summary>
    public event EventHandler<TimestampEventArgs> OnPing;

    /// <summary>
    /// Fired when a raw IRC message has been received from Twitch
    /// </summary>
    public event EventHandler<RawIrcMessageEventArgs> OnRawIrcMessage;

    /// <summary>
    /// Fired when the raw IRC message could not be parsed
    /// </summary>
    public event EventHandler<RawIrcMessageEventArgs> OnUnknownMessage;

    /// <summary>
    /// Fired when the raw IRC message was successfully parsed
    /// </summary>
    public event EventHandler<ParsedIrcMessageEventArgs> OnParsedIrcMessage;

    /// <summary>
    /// Fired when authentication failed (when a username and oath token is provided)
    /// </summary>
    public event EventHandler<TimestampEventArgs> OnAuthFailed;

    /// <summary>
    /// Fired when a exception it caught
    /// </summary>
    public event EventHandler<ExceptionEventArgs> OnException;

    /// <summary>
    /// Fired when a NOTICE command is received
    /// </summary>
    public event EventHandler<NoticeEventArgs> OnNotice;


    /// <summary>
    /// Fired when the bot connects to the Twitch Chat Network
    /// </summary>
    public event EventHandler<TimestampEventArgs> OnConnected;

    /// <summary>
    /// Fired when the bot disconnects from the Twitch Chat Network
    /// </summary>
    public event EventHandler<TimestampEventArgs> OnDisconnected;


    /// <summary>
    /// Fired when a channel has been joined
    /// </summary>
    public event EventHandler<ChannelJoinedEventArgs> OnChannelJoined;

    /// <summary>
    /// Fired when a channel has been parted
    /// </summary>
    public event EventHandler<ChannelPartedEventArgs> OnChannelParted;

    /// <summary>
    /// Fired when a user joins the channel
    /// </summary>
    public event EventHandler<UserJoinedChannelEventArgs> OnUserJoinedChannel;

    /// <summary>
    /// Fired when a user parts from the channel
    /// </summary>
    public event EventHandler<UserPartedChannelEventArgs> OnUserPartedChannel;

    /// <summary>
    /// Fired when a chat message is sent
    /// </summary>
    public event EventHandler<ChatMessageEventArgs> OnChatMessage;

    /// <summary>
    /// Fired when a whispered is sent to the bot, however, the bot requires the scopes <strong>whispers:read</strong> for this to work!
    /// </summary>
    public event EventHandler<WhisperEventArgs> OnWhisperMessage;

    /// <summary>
    /// Fired when a chat message is sent that contains bits
    /// </summary>
    public event EventHandler<ChatMessageEventArgs> OnBitsChatMessage;

    /// <summary>
    /// Fired when a elevated chat message is sent
    /// </summary>
    public event EventHandler<ElevatedMessageEventArgs> OnElevatedMessage;

    /// <summary>
    /// Fired when a viewer gets a milestone completed
    /// </summary>
    public event EventHandler<ViewerMilestoneMessageEventArgs> OnViewerMilestoneMessage;

    /// <summary>
    /// Fired when someone is raiding the channel
    /// </summary>
    public event EventHandler<RaidEventArgs> OnRaided;

    /// <summary>
    /// Fired when a raid gets canceled in a channel
    /// </summary>
    public event EventHandler<RaidCancelledEventArgs> OnRaidCancelled;

    /// <summary>
    /// Fired when the channel is hosting someone
    /// </summary>
    public event EventHandler<HostEventArgs> OnHosted;

    /// <summary>
    /// Fired after joining a channel and returns the list of usernames in the channel
    /// </summary>
    public event EventHandler<NamesListEventArgs> OnNamesList;


    /// <summary>
    /// Fired when someone subscribes for the first time
    /// </summary>
    public event EventHandler<SubscribeEventArgs> OnSubscribe;

    /// <summary>
    /// Fired when someone returns and resubscribes
    /// </summary>
    public event EventHandler<ResubscribeEventArgs> OnResubscribe;

    /// <summary>
    /// Fired when someone is gifted a subscription from someone
    /// </summary>
    public event EventHandler<GiftSubscriptionEventArgs> OnGiftSubscription;

    /// <summary>
    /// Fired when someone is gifting subscriptions to the community
    /// </summary>
    public event EventHandler<CommunityGiftSubscriptionEventArgs> OnCommunityGiftSubscription;

    /// <summary>
    /// Fired when someone chooses to continue a gift subscription they received from a known user
    /// </summary>
    public event EventHandler<ContinuingGiftSubscriptionEventArgs> OnContinuingGiftSubscription;

    /// <summary>
    /// Fired when someone chooses to upgrade their Prime subscription to a recurring subscription
    /// </summary>
    public event EventHandler<PrimeUpgradeSubscriptionEventArgs> OnPrimeUpgradeSubscription;

    /// <summary>
    /// Fired when someone chooses to continue a gift subscription they received from an anonymous user
    /// </summary>
    public event EventHandler<ContinuingAnonymousGiftSubscriptionEventArgs> OnContinuingAnonymousGiftSubscription;

    /// <summary>
    /// Fired when someone chooses to share their new bits badge
    /// </summary>
    public event EventHandler<BitsBadgeEventArgs> OnBitsBadge;

    /// <summary>
    /// Fired when someone is paying forward the gift subscription they were given by donating subscriptions to the community
    /// </summary>
    public event EventHandler<CommunityPayForwardEventArgs> OnCommunityPayForward;

    /// <summary>
    /// Fired when someone is paying forward the gift subscription they were given by giving a subscription to someone else directly
    /// </summary>
    public event EventHandler<SubscriptionPayForwardEventArgs> OnSubscriptionPayForward;

    /// <summary>
    /// Fired when a subscription is made. Helpful for when counting or doing something with subscriptions
    /// </summary>
    public event EventHandler<SubscriptionCounterEventArgs> OnSubscriptionCounter;


    /// <summary>
    /// Fired when someone is timed out
    /// </summary>
    public event EventHandler<UserTimeoutEventArgs> OnUserTimeout;

    /// <summary>
    /// Fired when someone is banned
    /// </summary>
    public event EventHandler<UserBannedEventArgs> OnUserBanned;

    /// <summary>
    /// Fired when the chat is cleared
    /// </summary>
    public event EventHandler<ChatClearedEventArgs> OnChatCleared;

    /// <summary>
    /// Fired when a chat message gets deleted
    /// </summary>
    public event EventHandler<ChatMessageDeletedEventArgs> OnChatMessageDeleted;


    /// <summary>
    /// Fired when the bot joins a channel and gets the state of the chat
    /// </summary>
    public event EventHandler<RoomStateEventArgs> OnRoomState;

    /// <summary>
    /// Fired when the emote only state is changed in the chat
    /// </summary>
    public event EventHandler<EmoteOnlyStateEventArgs> OnEmoteOnlyState;

    /// <summary>
    /// Fired when the followers only state is changed in the chat
    /// </summary>
    public event EventHandler<FollowersOnlyStateEventArgs> OnFollowersOnlyState;

    /// <summary>
    /// Fired when the slow mode state is changed in the chat
    /// </summary>
    public event EventHandler<SlowModeStateEventArgs> OnSlowModeState;

    /// <summary>
    /// Fired when the subscribers only state is changed in the chat
    /// </summary>
    public event EventHandler<SubscribersOnlyStateEventArgs> OnSubscribersOnlyState;

    #endregion


    /// <summary>
    /// Creates a new instance of <see cref="TwitchChatClient"/>. The bot is connected anonymously
    /// </summary>
    /// <param name="useSsl">True (default) to use SSL when connecting to Twitch Chat</param>
    public TwitchChatClient(bool useSsl = true) : this($"justinfan{Random.Shared.Next(100_000, 999_999)}", "", false, useSsl) { }


    /// <summary>
    /// Creates a new instance of <see cref="TwitchChatClient"/>.
    /// If you intend to send moderation commands the bot requires the scopes <strong>channel:moderate</strong> for that to work!
    /// </summary>
    /// <param name="username">Username of the Twitch account</param>
    /// <param name="oauth">OAuth token for the Twitch account</param>
    /// <param name="isModerator">If true, it indicates the bot can send 100 messages per 30 seconds, instead of 20 messages per 30 seconds</param>
    /// <param name="useSsl">True to use SSL when connecting to Twitch Chat. Default is true</param>
    /// <exception cref="ArgumentException">Cast upon invalid <paramref name="username"/></exception>
    public TwitchChatClient(string username, string oauth, bool isModerator = false, bool useSsl = true)
    {
        if (!UsernameRegex().IsMatch(username))
        {
            throw new ArgumentException("Invalid username provided!", nameof(username));
        }

        _joinedChannels = [];
        Username = username.ToLower();
        _oauth = oauth;
        _useSsl = useSsl;

        IsAnonymous = username.StartsWith("justinfan") && string.IsNullOrEmpty(oauth);

        // One extra second is added, just in case...
        _joinQueue = new WaitableConcurrentQueue<string>(20, TimeSpan.FromSeconds(11));
        _messageQueue = new WaitableConcurrentQueue<string>(isModerator ? 100 : 20, TimeSpan.FromSeconds(31));
        _anonymousCommunityGiftIds = new LimitedCollection<string>(100);

        _reconnectCooldown = new Cooldown(TimeSpan.FromSeconds(60), 1, 8);
        _shouldConnectRefresher = new Refresher(TimeSpan.FromSeconds(15));

        _monitorTimer = new Timer(MonitoringLogic, null, Timeout.Infinite, Timeout.Infinite);
    }


    /// <summary>
    /// Connects to the Twitch Chat Network
    /// </summary>
    public async Task<bool> ConnectAsync()
    {
        if (IsConnected || _isDisposing)
        {
            return false;
        }

        HelperHandleLog("Connection started...");
        return await InternalConnectAsync();
    }


    /// <summary>
    /// Reconnect to the Twitch Chat Network, also rejoins channels if <see cref="RememberChannelsOnReconnect" /> is true
    /// </summary>
    public async Task<bool> ReconnectAsync()
    {
        if (_isDisposing)
        {
            return false;
        }

        HelperHandleLog("Trying to reconnect...");
        var ok = await InternalConnectAsync();

        _joinQueue.Clear();

        if (!RememberChannelsOnReconnect)
        {
            HelperHandleLog("Clearing joined channels...");
            _joinedChannels.Clear();
            return ok;
        }

        if (!ok)
        {
            return false;
        }

        for (int i = 0; i < _joinedChannels.Count; i++)
        {
            await JoinChannelAsync(_joinedChannels[i], true);
        }

        return true;
    }


    /// <summary>
    /// Closes the connection to the Twitch Chat Network
    /// </summary>
    /// <param name="updateState">
    /// Determines if the internal state will be updated (default is true).
    /// Example, if you set this to false and have <see cref="AutoReconnect"/> enabled the instance will see this as a disconnect and try to reconnect!
    /// </param>
    public async Task DisconnectAsync(bool updateState = true)
    {
        try
        {
            if (updateState)
            {
                // We chose to disconnect
                _shouldBeConnected = false;
            }

            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Manual disconnect", CancellationToken.None);
        }
        catch { }
    }


    /// <summary>
    /// A helper for waiting until the client is connected to the Twitch Chat Network
    /// </summary>
    /// <param name="maxWaitTime">If null, it can wait forever</param>
    public async Task WaitUntilConnected(TimeSpan? maxWaitTime)
    {
        var start = DateTime.Now;

        while (true)
        {
            if (IsConnected)
            {
                return;
            }

            if (maxWaitTime != null && DateTime.Now - start >= maxWaitTime)
            {
                return;
            }

            await Task.Delay(100);
        }
    }


    /// <summary>
    /// Internal logic for connecting to the Twitch Chat Network
    /// </summary>
    private async Task<bool> InternalConnectAsync()
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        _client?.Abort();
        _client?.Dispose();

        _shouldBeConnected = true;

        // Wait 5 sec, then start the monitor timer loop
        _monitorTimer.Change(5000, Timeout.Infinite);

        try
        {
            _client = new ClientWebSocket();
            _client.Options.KeepAliveInterval = TimeSpan.FromSeconds(10);

            _tokenSource = new CancellationTokenSource();

            var url = _useSsl
                ? $"wss://{HOST}:443"
                : $"ws://{HOST}:80";

            await _client.ConnectAsync(new Uri(url), _tokenSource.Token);

            // Wait a bit
            await Task.Delay(250, _tokenSource.Token);

            BeginListening();

            // Wait a bit
            await Task.Delay(250, _tokenSource.Token);

            await SendAsync("CAP REQ twitch.tv/membership");
            await SendAsync("CAP REQ twitch.tv/commands");
            await SendAsync("CAP REQ twitch.tv/tags");

            // Wait a bit
            await Task.Delay(250, _tokenSource.Token);

            RaiseConnected();

            await SendAsync($"PASS oauth:{_oauth}");
            await SendAsync($"NICK {Username}");

            // To give it some time
            await Task.Delay(250, _tokenSource.Token);

            BeginJoinQueue();
            BeginMessageQueue();

            return true;
        }
        catch (Exception ex)
        {
            RaiseDisconnected();
            _reconnectPending = false;

            if (ex is not WebSocketException && ex is not OperationCanceledException)
            {
                HelperHandleExceptions(ex);
            }

            return false;
        }
    }


    /// <summary>
    /// Joins a Twitch channel
    /// </summary>
    /// <param name="channel">Channel name to join</param>
    /// <param name="bypassCheck">True to bypass the check if already joined. Default is false</param>
    /// <param name="bypassQueue">True to bypass the message queue (20 commands per 10 seconds). Default is false</param>
    /// <exception cref="ArgumentException">Cast upon invalid <paramref name="channel"/></exception>
    public async Task JoinChannelAsync(string channel, bool bypassCheck = false, bool bypassQueue = false)
    {
        if (_isDisposing)
        {
            return;
        }

        if (!UsernameRegex().IsMatch(channel))
        {
            throw new ArgumentException("Invalid channel name provided!", nameof(channel));
        }

        channel = channel.ToLower();
        HelperHandleLog($"Joining channel '{channel}'...");

        if (!bypassCheck && _joinedChannels.Contains(channel))
        {
            HelperHandleLog($"Already in channel '{channel}'!");
            return;
        }

        if (!IsConnected)
        {
            HelperHandleLog($"Not connected! Adding channel '{channel}' to list instead.");

            // Attempt rejoin on reconnect instead
            if (!_joinedChannels.Contains(channel))
            {
                _joinedChannels.Add(channel);
            }

            return;
        }

        var cmd = $"JOIN #{channel}";

        if (bypassQueue)
        {
            await SendAsync(cmd);
            return;
        }

        _joinQueue.Enqueue(cmd);
    }


    /// <summary>
    /// Parts a Twitch channel
    /// </summary>
    /// <param name="channel">Channel name to part</param>
    /// <param name="bypassQueue">True to bypass the message queue (20 commands per 10 seconds). Default is false</param>
    /// <exception cref="ArgumentException">Cast upon invalid <paramref name="channel"/></exception>
    public async Task PartChannelAsync(string channel, bool bypassQueue = false)
    {
        if (_isDisposing)
        {
            return;
        }

        if (!UsernameRegex().IsMatch(channel))
        {
            throw new ArgumentException("Invalid channel name provided!", nameof(channel));
        }

        channel = channel.ToLower();
        HelperHandleLog($"Parting channel '{channel}'...");

        if (!_joinedChannels.Contains(channel))
        {
            HelperHandleLog($"Not in channel '{channel}'!");
            return;
        }

        if (!IsConnected)
        {
            HelperHandleLog($"Not connected! Removing channel '{channel}' to list instead.");

            _joinedChannels.Remove(channel);
            return;
        }

        var cmd = $"PART #{channel}";

        if (bypassQueue)
        {
            await SendAsync(cmd);
            return;
        }

        _joinQueue.Enqueue(cmd);
    }


    /// <summary>
    /// Sends a message in a channel.
    /// The bot requires the scopes <strong>chat:edit</strong> for this to work
    /// </summary>
    /// <param name="channel">Channel to send message to</param>
    /// <param name="message">The message to send</param>
    /// <param name="replyId">The message ID to reply too. Leave it as null/empty to send a normal message.</param>
    /// <param name="bypassQueue">True to bypass the message queue. Default is false</param>
    /// <exception cref="ArgumentException">Cast upon invalid <paramref name="channel"/></exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="TwitchChatClient"/> is connected anonymously</exception>
    public async Task SendMessageAsync(string channel, string message, string replyId = null, bool bypassQueue = false)
    {
        if (_isDisposing)
        {
            return;
        }

        if (IsAnonymous)
        {
            HelperHandleLog("Client is currently in Read-Only mode, please provide your bot's username and OAuth-token to be able to send chat messages.");
            return;
        }

        if (!UsernameRegex().IsMatch(channel))
        {
            throw new ArgumentException("Invalid channel name provided!", nameof(channel));
        }

        channel = channel.ToLower();
        HelperHandleLog($"Sending chat message '{message}' to channel '{channel}' (ID= {replyId})...");

        var cmd = $"PRIVMSG #{channel} :{message}";

        if (!string.IsNullOrEmpty(replyId))
        {
            cmd = $"@{KnownTags.REPLY_PARENT_MSG_ID}={replyId} {cmd}";
        }

        if (bypassQueue)
        {
            await SendAsync(cmd);
            return;
        }

        _messageQueue.Enqueue(cmd);
    }


    /// <summary>
    /// Sends a whisper to a user.
    /// The bot requires the scopes <strong>whispers:edit</strong> and to be a <u>KNOWN BOT</u> for this to work.
    /// More info: https://discuss.dev.twitch.tv/t/my-bot-cant-send-a-whisp/21481
    /// </summary>
    /// <param name="username">The username to whisper to</param>
    /// <param name="message">The message to send</param>
    /// <param name="bypassQueue">True to bypass the message queue. Default is false</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="TwitchChatClient"/> is connected anonymously</exception>
    public async Task SendWhisperAsync(string username, string message, bool bypassQueue = false)
    {
        if (_isDisposing)
        {
            return;
        }

        if (IsAnonymous)
        {
            HelperHandleLog("Client is currently in Read-Only mode, please provide your bot's username and OAuth-token to be able to send whisper messages.");
            return;
        }

        if (!UsernameRegex().IsMatch(username))
        {
            throw new ArgumentException("Invalid username provided!", nameof(username));
        }

        username = username.ToLower();
        HelperHandleLog($"Sending whisper message '{message}' to '{username}'");

        var cmd = $":{username}!{username}@{username}.tmi.twitch.tv WHISPER {Username} :{message}";

        if (bypassQueue)
        {
            await SendAsync(cmd);
            return;
        }

        _messageQueue.Enqueue(cmd);
    }


    /// <summary>
    /// Primarily used for testing
    /// </summary>
    /// <param name="rawIrcMessagesToParse">Raw IRC message to simulate</param>
    public async Task SimulateMessagesAsync(params string[] rawIrcMessagesToParse)
    {
        if (_isDisposing)
        {
            return;
        }

        HelperHandleLog($"Sending {rawIrcMessagesToParse.Length} simulated messages...");

        foreach (var rawIrcMessages in rawIrcMessagesToParse.Select(a => a.Trim()))
        {
            OnRawIrcMessage?.Invoke(this, new RawIrcMessageEventArgs(rawIrcMessages));

            if (rawIrcMessages == "###")
            {
                Console.WriteLine("");
                continue;
            }
            else if (rawIrcMessages == "---")
            {
                Console.WriteLine(new string('-', 64));
                continue;
            }

            var ircEntity = ParsedIrcMessage.Parse(rawIrcMessages, KeepRawIrcMessageAfterParsed);
            if (ircEntity == null)
            {
                OnUnknownMessage?.Invoke(this, new RawIrcMessageEventArgs(rawIrcMessages, string.Empty, "(Simulate) Message could not be parsed!"));
                continue;
            }

            OnParsedIrcMessage?.Invoke(this, new ParsedIrcMessageEventArgs(ircEntity));

            await ProcessParsedIrcMessageAsync(rawIrcMessages, ircEntity, CancellationToken.None);
        }
    }


    /// <summary>
    /// Raises the <see cref="OnConnected"/> event and executes logic around it.
    /// </summary>
    private void RaiseConnected()
    {
        OnConnected?.Invoke(this, new TimestampEventArgs());

        _canFireDisconnect = true;
        _reconnectPending = false;
    }


    /// <summary>
    /// Raises the <see cref="OnDisconnected"/> event and executes logic around it.
    /// </summary>
    private void RaiseDisconnected()
    {
        if (_canFireDisconnect)
        {
            OnDisconnected?.Invoke(this, new TimestampEventArgs());
            _canFireDisconnect = false;
        }

        if (_isDisposing || !_shouldBeConnected || !AutoReconnect || _reconnectPending)
        {
            return;
        }

        _reconnectPending = true;

        // Increase counter
        var waitCount = _reconnectCooldown.GetAndIncrease();
        var seconds = 5 * waitCount;
        _tryReconnectAfter = DateTime.Now.AddSeconds(seconds);

        HelperHandleLog($"Connection lost! Try after {seconds} seconds at {_tryReconnectAfter}. FailedConnects = {waitCount}");
    }


    /// <summary>
    /// Sends a raw IRC message
    /// </summary>
    /// <param name="rawIrcMessage">Raw IRC message to send</param>
    /// <param name="token">A cancellation token. Default is null</param>
    private async Task SendAsync(string rawIrcMessage, CancellationToken? token = null)
    {
        if (!IsConnected || _isDisposing)
        {
            HelperHandleLog("SendAsync ignored! Not connected or already disposed!");
            return;
        }

        HelperHandleLog($"Sending raw IRC message: {rawIrcMessage}");

        await _client.SendAsync(
            new ArraySegment<byte>(Helper.GetBytes(rawIrcMessage)),
            WebSocketMessageType.Text,
            true,
            token ?? _tokenSource.Token
        );
    }


    /// <summary>
    /// Listener loop, handles the incoming raw IRC messages and parses them
    /// </summary>
    private void BeginListening()
    {
        Task.Factory.StartNew(async () =>
        {
            try
            {
                var message = "";
                var lines = Array.Empty<string>();
                var buffer = new byte[1024];

                HelperHandleLog("BeginListening loop started!");

                while (!_tokenSource.Token.IsCancellationRequested && IsConnected)
                {
                    try
                    {
                        Array.Clear(buffer);
                        var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), _tokenSource.Token);
                        if (result == null)
                        {
                            continue;
                        }

                        switch (result.MessageType)
                        {
                            case WebSocketMessageType.Close:
                                RaiseDisconnected();
                                return;
                            case WebSocketMessageType.Text when !result.EndOfMessage:
                                message += Helper.GetString(buffer).TrimEnd('\0');
                                continue; // There is more data!
                            case WebSocketMessageType.Text:
                                message += Helper.GetString(buffer).TrimEnd('\0');
                                break;
                            case WebSocketMessageType.Binary:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        lines = message.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                        foreach (var line in lines)
                        {
                            OnRawIrcMessage?.Invoke(this, new RawIrcMessageEventArgs(line));

                            var ircEntity = ParsedIrcMessage.Parse(line, KeepRawIrcMessageAfterParsed);
                            if (ircEntity == null)
                            {
                                OnUnknownMessage?.Invoke(this, new RawIrcMessageEventArgs(line, string.Empty, "Message could not be parsed!"));
                                continue;
                            }

                            if (ircEntity.Code == KnownCommands.NOTICE && ircEntity.Message == Constants.LOGIN_AUTH_FAILED)
                            {
                                _shouldBeConnected = false;
                                OnAuthFailed?.Invoke(this, new TimestampEventArgs());
                                await DisposeAsync();
                                RaiseDisconnected();
                                return;
                            }

                            OnParsedIrcMessage?.Invoke(this, new ParsedIrcMessageEventArgs(ircEntity));

                            await ProcessParsedIrcMessageAsync(line, ircEntity, _tokenSource.Token);
                        }

                        message = "";
                    }
                    catch (TaskCanceledException) { }
                    catch (OperationCanceledException) { }
                }
            }
            catch (Exception ex)
            {
                if (ex is not WebSocketException)
                {
                    HelperHandleExceptions(ex);
                }

                RaiseDisconnected();
            }
        }, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }


    /// <summary>
    /// Channel join/part queue, handles the JOIN/PART messages and sends them according to the queue rules
    /// </summary>
    private void BeginJoinQueue()
    {
        Task.Factory.StartNew(async () =>
        {
            try
            {
                HelperHandleLog("BeginJoinQueue loop started!");

                while (!_tokenSource.Token.IsCancellationRequested && IsConnected)
                {
                    try
                    {
                        var queuedJoin = await _joinQueue.DequeueAsync(_tokenSource.Token);

                        if (string.IsNullOrEmpty(queuedJoin))
                        {
                            HelperHandleLog("Join command was empty/null");
                            continue;
                        }

                        await SendAsync(queuedJoin, _tokenSource.Token);
                    }
                    catch (TaskCanceledException) { }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        HelperHandleExceptions(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                HelperHandleExceptions(ex);

                RaiseDisconnected();
            }
        }, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }


    /// <summary>
    /// Channel message queue, handles the chat messages and sends them according to the queue rules
    /// </summary>
    private void BeginMessageQueue()
    {
        Task.Factory.StartNew(async () =>
        {
            try
            {
                HelperHandleLog("BeginMessageQueue loop started!");

                while (!_tokenSource.Token.IsCancellationRequested && IsConnected)
                {
                    try
                    {
                        var queuedMessage = await _messageQueue.DequeueAsync(_tokenSource.Token);

                        if (string.IsNullOrEmpty(queuedMessage))
                        {
                            HelperHandleLog("Message command was empty/null");
                            continue;
                        }

                        await SendAsync(queuedMessage, _tokenSource.Token);
                    }
                    catch (TaskCanceledException) { }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        HelperHandleExceptions(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                HelperHandleExceptions(ex);

                RaiseDisconnected();
            }
        }, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }


    /// <summary>
    /// Monitor logic, keeps <see cref="TwitchChatClient"/> connected to the Twitch Chat Network (if it should)
    /// </summary>
    private async void MonitoringLogic(object _)
    {
        if (_isDisposing)
        {
            // Cancel loop
            return;
        }

        try
        {
#if DEBUG
            Console.Title = "Monitor " + DateTime.Now;
#endif
            if (_tryReconnectAfter != null && DateTime.Now > _tryReconnectAfter)
            {
                // Clear value, we are trying to reconnect
                _tryReconnectAfter = null;

                await ReconnectAsync();
            }
            else if (!IsConnected && _shouldBeConnected && _shouldConnectRefresher.GetAndIncrease() == 0)
            {
                RaiseDisconnected();
            }

            if (MonitorLogModular > 0 && DateTime.Now.Minute % MonitorLogModular == 0)
            {
                HelperHandleLog($"IsConnected = {IsConnected} | ShouldBeConnected = {_shouldBeConnected} | JoinedChannels = {string.Join(", ", _joinedChannels)}");
            }
        }
        catch (Exception ex)
        {
            HelperHandleExceptions(ex);
        }

        // Wait 1 sec, then run again
        _monitorTimer.Change(1000, Timeout.Infinite);
    }


    /// <summary>
    /// Process the parsed IRC messages
    /// </summary>
    /// <param name="rawIrcMessage">The raw IRC message</param>
    /// <param name="ircEntity">The parsed IRC message</param>
    /// <param name="token">A cancellation token</param>
    private async Task ProcessParsedIrcMessageAsync(string rawIrcMessage, ParsedIrcMessage ircEntity, CancellationToken token)
    {
        if (ircEntity.Code == KnownCommands.PRIVMSG)
        {
            if (OnChatMessage == null && OnBitsChatMessage == null)
            {
                // No events, do nothing
                return;
            }

            var bits = ircEntity.Tags.GetIntValue(KnownTags.BITS);

            var lazyEventArgs = new Lazy<ChatMessageEventArgs>(() =>
            {
                return new ChatMessageEventArgs(
                    IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                    ircEntity.Timestamp,
                    ircEntity.Channel,
                    ircEntity.Username,
                    ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                    ircEntity.Message,
                    ParseEmotesInMessages ? ircEntity.Tags.GetStringValue(KnownTags.EMOTES) : null,
                    bits,
                    ircEntity.Tags.GetStringValue(KnownTags.ID),
                    ircEntity.Tags.GetStringValue(KnownTags.BADGES),
                    ircEntity.Tags.GetStringValue(KnownTags.MSG_ID),
                    ircEntity.Tags.GetStringValue(KnownTags.CUSTOM_REWARD_ID),
                    ircEntity.Tags.ContainsKey(KnownTags.YATMI_IS_ME),
                    ircEntity.Tags.GetStringValue(KnownTags.FIRST_MSG) == "1",
                    ircEntity.Tags.GetStringValue(KnownTags.RETURNING_CHATTER) == "1",
                    PinnedChatPaidEntity.TryCreate(ircEntity.Tags)
                );
            });

            OnChatMessage?.Invoke(this, lazyEventArgs.Value);

            if (OnBitsChatMessage != null && bits > 0)
            {
                OnBitsChatMessage?.Invoke(this, lazyEventArgs.Value);
            }
        }
        else if (ircEntity.Code == KnownCommands.USERNOTICE)
        {
            ProcessUserNotice(rawIrcMessage, ircEntity);
        }
        else if (ircEntity.Code == KnownCommands.USERNOTICE)
        {
            ProcessUserNotice(rawIrcMessage, ircEntity);
        }
        else if (ircEntity.Code == KnownCommands.CLEARCHAT || ircEntity.Code == KnownCommands.CLEARMSG)
        {
            ProcessModeration(ircEntity);
        }
        else if (ircEntity.Code == KnownCommands.WHISPER)
        {
            OnWhisperMessage?.Invoke(this, new WhisperEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Username,
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Message
            ));
        }
        else if (ircEntity.Code == KnownCommands.ROOMSTATE)
        {
            ProcessRoomState(rawIrcMessage, ircEntity);
        }
        else if (ircEntity.Code == KnownCommands.NOTICE)
        {
            await ProcessNoticeAsync(ircEntity);
        }
        else if (ircEntity.Code == KnownCommands.HOSTTARGET)
        {
            OnHosted?.Invoke(this, new HostEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.YATMI_HOST_TARGET),
                ircEntity.Tags.GetIntValue(KnownTags.YATMI_HOST_VIEWERS)
            ));
        }
        else if (ircEntity.Code == KnownCommands.JOIN)
        {
            if (ircEntity.Username == Username)
            {
                if (!_joinedChannels.Contains(ircEntity.Channel))
                {
                    _joinedChannels.Add(ircEntity.Channel);
                }

                OnChannelJoined?.Invoke(this, new ChannelJoinedEventArgs(
                    IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                    ircEntity.Timestamp,
                    ircEntity.Channel
                ));
            }
            else
            {
                OnUserJoinedChannel?.Invoke(this, new UserJoinedChannelEventArgs(
                    IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                    ircEntity.Timestamp,
                    ircEntity.Channel,
                    ircEntity.Username
                ));
            }
        }
        else if (ircEntity.Code == KnownCommands.PART)
        {
            if (ircEntity.Username == Username)
            {
                _joinedChannels.Remove(ircEntity.Channel);

                OnChannelParted?.Invoke(this, new ChannelPartedEventArgs(
                    IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                    ircEntity.Timestamp,
                    ircEntity.Channel
                ));
            }
            else
            {
                OnUserPartedChannel?.Invoke(this, new UserPartedChannelEventArgs(
                    IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                    ircEntity.Timestamp,
                    ircEntity.Channel,
                    ircEntity.Username
                ));
            }
        }
        else if (ircEntity.Code == KnownCommands.PING)
        {
            OnPing?.Invoke(this, new TimestampEventArgs());

            // Respond to ping!
            await SendAsync(ircEntity.Message, token);
        }
        else if (ircEntity.Code == KnownCommands.RECONNECT && IsConnected)
        {
            RaiseDisconnected();
        }
        else if (ircEntity.Code == KnownCommands.NAMES_LIST && ircEntity.Message != Username)
        {
            OnNamesList?.Invoke(this, new NamesListEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Message
            ));
        }
    }


    /// <summary>
    /// Process the <see cref="KnownCommands.USERNOTICE"/> messages
    /// </summary>
    /// <param name="rawIrcMessage">The raw IRC message</param>
    /// <param name="ircEntity">The parsed IRC message</param>
    private void ProcessUserNotice(string rawIrcMessage, ParsedIrcMessage ircEntity)
    {
        var msgId = ircEntity.Tags.GetStringValue(KnownTags.MSG_ID);

        if (msgId == KnownMessageIds.ANNOUNCEMENT)
        {
            OnChatMessage?.Invoke(this, new ChatMessageEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Message,
                ParseEmotesInMessages ? ircEntity.Tags.GetStringValue(KnownTags.EMOTES) : null,
                ircEntity.Tags.GetIntValue(KnownTags.BITS),
                ircEntity.Tags.GetStringValue(KnownTags.ID),
                ircEntity.Tags.GetStringValue(KnownTags.BADGES),
                KnownMessageIds.ANNOUNCEMENT,
                null,
                ircEntity.Tags.ContainsKey(KnownTags.YATMI_IS_ME),
                ircEntity.Tags.GetStringValue(KnownTags.FIRST_MSG) == "1",
                ircEntity.Tags.GetStringValue(KnownTags.RETURNING_CHATTER) == "1",
                PinnedChatPaidEntity.TryCreate(ircEntity.Tags) // Can't be announcement and paid to my knowledge, but just in case.
            ));
        }
        else if (msgId == KnownMessageIds.SUB)
        {
            if (OnSubscribe == null && OnSubscriptionCounter == null)
            {
                // No events, do nothing
                return;
            }

            var eventArgs = new SubscribeEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetIntValue(KnownTags.MSG_PARAM_CUMULATIVE_MONTHS),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_SUB_PLAN),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG)
            );

            OnSubscribe?.Invoke(this, eventArgs);
            OnSubscriptionCounter?.Invoke(this, SubscriptionCounterEventArgs.From(eventArgs));
        }
        else if (msgId == KnownMessageIds.RESUB)
        {
            if (OnResubscribe == null && OnSubscriptionCounter == null)
            {
                // No events, do nothing
                return;
            }

            var eventArgs = new ResubscribeEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Message,
                ParseEmotesInMessages ? ircEntity.Tags.GetStringValue(KnownTags.EMOTES) : null,
                ircEntity.Tags.GetIntValue(KnownTags.MSG_PARAM_CUMULATIVE_MONTHS),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_SUB_PLAN),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG)
            );

            OnResubscribe?.Invoke(this, eventArgs);
            OnSubscriptionCounter?.Invoke(this, SubscriptionCounterEventArgs.From(eventArgs));
        }
        else if (msgId == KnownMessageIds.SUBGIFT)
        {
            if (OnGiftSubscription == null && OnSubscriptionCounter == null)
            {
                // No events, do nothing
                return;
            }

            var lazyEventArgs = new Lazy<GiftSubscriptionEventArgs>(() =>
            {
                var fromAnonymousCommunityGift = false;
                var login = ircEntity.Tags.GetStringValue(KnownTags.LOGIN);
                if (login == KnownLogins.AN_ANONYMOUS_GIFTER)
                {
                    fromAnonymousCommunityGift = _anonymousCommunityGiftIds.Contains(ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_MASS_ORIGIN_ID));
                }

                return new GiftSubscriptionEventArgs(
                    IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                    ircEntity.Timestamp,
                    ircEntity.Channel,
                    login,
                    ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                    ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_RECIPIENT_USERNAME),
                    ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_RECIPIENT_USER_ID),
                    ircEntity.Tags.GetIntValue(KnownTags.MSG_PARAM_SENDER_COUNT),
                    fromAnonymousCommunityGift,
                    ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_SUB_PLAN),
                    ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG)
                );
            });

            OnGiftSubscription?.Invoke(this, lazyEventArgs.Value);

            if (OnSubscriptionCounter != null && lazyEventArgs.Value.SubGiftType == SubGiftTypes.PersonalGift)
            {
                OnSubscriptionCounter?.Invoke(this, SubscriptionCounterEventArgs.From(lazyEventArgs.Value));
            }
        }
        else if (msgId == KnownMessageIds.COMMUNITY_PAY_FORWARD)
        {
            OnCommunityPayForward?.Invoke(this, new CommunityPayForwardEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_GIFTER_USERNAME),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_GIFTER_USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG)
            ));
        }
        else if (msgId == KnownMessageIds.STANDARD_PAY_FORWARD)
        {
            OnSubscriptionPayForward?.Invoke(this, new SubscriptionPayForwardEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_GIFTER_USERNAME),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_GIFTER_USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_RECIPIENT_USERNAME),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_RECIPIENT_USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG)
            ));
        }
        else if (msgId == KnownMessageIds.MYSTERY_SUBGIFT)
        {
            if (OnCommunityGiftSubscription == null && OnSubscriptionCounter == null)
            {
                // No events, do nothing
                return;
            }

            var login = ircEntity.Tags.GetStringValue(KnownTags.LOGIN);
            if (login == KnownLogins.AN_ANONYMOUS_GIFTER)
            {
                // To keep track if anonymous subscription is from a Gift or Directly
                _anonymousCommunityGiftIds.Add(ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_MASS_ORIGIN_ID));
            }

            var eventArgs = new CommunityGiftSubscriptionEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                login,
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetIntValue(KnownTags.MSG_PARAM_MASS_GIFT_COUNT),
                ircEntity.Tags.GetIntValue(KnownTags.MSG_PARAM_SENDER_COUNT),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_SUB_PLAN),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG)
            );

            OnCommunityGiftSubscription?.Invoke(this, eventArgs);
            OnSubscriptionCounter?.Invoke(this, SubscriptionCounterEventArgs.From(eventArgs));
        }
        else if (msgId == KnownMessageIds.GIFT_PAID_UPGRADE)
        {
            OnContinuingGiftSubscription?.Invoke(this, new ContinuingGiftSubscriptionEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_SENDER_LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG)
            ));
        }
        else if (msgId == KnownMessageIds.PRIME_PAID_UPGRADE)
        {
            OnPrimeUpgradeSubscription?.Invoke(this, new PrimeUpgradeSubscriptionEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_SUB_PLAN),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG)
            ));
        }
        else if (msgId == KnownMessageIds.ANON_GIFT_PAID_UPGRADE)
        {
            OnContinuingAnonymousGiftSubscription?.Invoke(this, new ContinuingAnonymousGiftSubscriptionEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG)
            ));
        }
        else if (msgId == KnownMessageIds.BITS_BADGE_TIER)
        {
            OnBitsBadge?.Invoke(this, new BitsBadgeEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Message,
                ircEntity.Tags.GetStringValue(KnownTags.DISPLAY_NAME),
                ircEntity.Tags.GetIntValue(KnownTags.MSG_PARAM_THRESHOLD)
            ));
        }
        else if (msgId == KnownMessageIds.RAID)
        {
            OnRaided?.Invoke(this, new RaidEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetIntValue(KnownTags.MSG_PARAM_VIEWER_COUNT)
            ));
        }
        else if (msgId == KnownMessageIds.UNRAID)
        {
            OnRaidCancelled?.Invoke(this, new RaidCancelledEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG)
            ));
        }
        else if (msgId == KnownMessageIds.MIDNIGHTSQUID)
        {
            OnElevatedMessage?.Invoke(this, new ElevatedMessageEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_AMOUNT),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_CURRENCY)
            ));
        }
        else if (msgId == KnownMessageIds.VIEWERMILESTONE)
        {
            OnViewerMilestoneMessage?.Invoke(this, new ViewerMilestoneMessageEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Tags.GetStringValue(KnownTags.SYSTEM_MSG),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_CATEGORY),
                ircEntity.Tags.GetStringValue(KnownTags.MSG_PARAM_VALUE)
            ));
        }
        else
        {
            OnUnknownMessage?.Invoke(this, new RawIrcMessageEventArgs(rawIrcMessage, KnownCommands.USERNOTICE, "MsgID was unknown or missing"));
        }
    }


    /// <summary>
    /// Process the <see cref="KnownCommands.ROOMSTATE"/> messages
    /// </summary>
    /// <param name="rawIrcMessage">The raw IRC message</param>
    /// <param name="ircEntity">The parsed IRC message</param>
    private void ProcessRoomState(string rawIrcMessage, ParsedIrcMessage ircEntity)
    {
        var hasEmoteOnlyTag = ircEntity.Tags.ContainsKey(KnownTags.EMOTES_ONLY);
        var hasFollowersOnlyTag = ircEntity.Tags.ContainsKey(KnownTags.FOLLOWERS_ONLY);
        var hasSlowTag = ircEntity.Tags.ContainsKey(KnownTags.SLOW);
        var hasSubOnlyTag = ircEntity.Tags.ContainsKey(KnownTags.SUBS_ONLY);

        if (hasEmoteOnlyTag && hasFollowersOnlyTag && hasSlowTag && hasSubOnlyTag)
        {
            OnRoomState?.Invoke(this, new RoomStateEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetIntValue(KnownTags.EMOTES_ONLY),
                ircEntity.Tags.GetIntValue(KnownTags.FOLLOWERS_ONLY),
                ircEntity.Tags.GetIntValue(KnownTags.SLOW),
                ircEntity.Tags.GetIntValue(KnownTags.SUBS_ONLY)
            ));
        }
        else if (hasEmoteOnlyTag)
        {
            OnEmoteOnlyState?.Invoke(this, new EmoteOnlyStateEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetIntValue(KnownTags.EMOTES_ONLY)
            ));
        }
        else if (hasFollowersOnlyTag)
        {
            OnFollowersOnlyState?.Invoke(this, new FollowersOnlyStateEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetIntValue(KnownTags.FOLLOWERS_ONLY)
            ));
        }
        else if (hasSlowTag)
        {
            OnSlowModeState?.Invoke(this, new SlowModeStateEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetIntValue(KnownTags.SLOW)
            ));
        }
        else if (hasSubOnlyTag)
        {
            OnSubscribersOnlyState?.Invoke(this, new SubscribersOnlyStateEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetIntValue(KnownTags.SUBS_ONLY)
            ));
        }
        else
        {
            OnUnknownMessage?.Invoke(this, new RawIrcMessageEventArgs(rawIrcMessage, KnownCommands.ROOMSTATE, "None of the tags for the room state was found"));
        }
    }


    /// <summary>
    /// Process the <see cref="KnownCommands.CLEARCHAT"/> or <see cref="KnownCommands.CLEARMSG"/> messages
    /// </summary>
    /// <param name="ircEntity">The parsed IRC message</param>
    private void ProcessModeration(ParsedIrcMessage ircEntity)
    {
        if (ircEntity.Code == KnownCommands.CLEARCHAT)
        {
            var hasTargetUserID = ircEntity.Tags.ContainsKey(KnownTags.TARGET_USER_ID);
            if (hasTargetUserID)
            {
                var timeoutDuration = ircEntity.Tags.GetIntValue(KnownTags.BAN_DURATION);

                if (timeoutDuration > 0)
                {
                    OnUserTimeout?.Invoke(this, new UserTimeoutEventArgs(
                        IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                        ircEntity.Timestamp,
                        ircEntity.Channel,
                        ircEntity.Message,
                        ircEntity.Tags.GetStringValue(KnownTags.TARGET_USER_ID),
                        timeoutDuration
                    ));
                }
                else
                {
                    OnUserBanned?.Invoke(this, new UserBannedEventArgs(
                        IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                        ircEntity.Timestamp,
                        ircEntity.Channel,
                        ircEntity.Message,
                        ircEntity.Tags.GetStringValue(KnownTags.TARGET_USER_ID)
                    ));
                }
            }
            else
            {
                OnChatCleared?.Invoke(this, new ChatClearedEventArgs(
                    IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                    ircEntity.Timestamp,
                    ircEntity.Channel
                ));
            }
        }
        else if (ircEntity.Code == KnownCommands.CLEARMSG)
        {
            OnChatMessageDeleted?.Invoke(this, new ChatMessageDeletedEventArgs(
                IncludeParsedIrcMessagesInEvents ? ircEntity : null,
                ircEntity.Timestamp,
                ircEntity.Channel,
                ircEntity.Tags.GetStringValue(KnownTags.LOGIN),
                ircEntity.Tags.GetStringValue(KnownTags.USER_ID),
                ircEntity.Message
            ));
        }
    }


    /// <summary>
    /// Process the <see cref="KnownCommands.NOTICE"/> messages
    /// </summary>
    /// <param name="ircEntity">The parsed IRC message</param>
    private async Task ProcessNoticeAsync(ParsedIrcMessage ircEntity)
    {
        var msgId = ircEntity.Tags.GetStringValue(KnownTags.MSG_ID);

        if (msgId == KnownMessageIds.MSG_CHANNEL_SUSPENDED)
        {
            await PartChannelAsync(ircEntity.Channel);
        }

        var eventArgs = new NoticeEventArgs(
            IncludeParsedIrcMessagesInEvents ? ircEntity : null,
            ircEntity.Timestamp,
            ircEntity.Channel,
            ircEntity.Message,
            msgId
        );

        OnNotice?.Invoke(this, eventArgs);
    }


    /// <summary>
    /// Helper for handling the caught exceptions
    /// </summary>
    /// <param name="ex">Caught exception</param>
    /// <param name="memberName">Method that this was called from</param>
    private void HelperHandleExceptions(Exception ex, [CallerMemberName] string memberName = null)
    {
        OnException?.Invoke(this, new ExceptionEventArgs(ex));
#if DEBUG
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"--------------------------{memberName}------------------------------");
        Console.WriteLine($"[{ex.GetType().FullName}]");
        Console.WriteLine(ex);
        Console.WriteLine($"--------------------------{memberName}------------------------------");
        Console.ResetColor();
#endif
    }


    /// <summary>
    /// Helper for handling logging messages
    /// </summary>
    /// <param name="message">The message to log</param>
    private void HelperHandleLog(string message)
    {
        OnLog?.Invoke(this, new LogEventArgs(IsAnonymous
            ? message
            : message.Replace(_oauth, "***")
        ));
#if DEBUG
        System.Diagnostics.Debug.WriteLine(message);
#endif
    }


    /// <summary>
    /// Disconnects from the Twitch Chat Network and releases resources
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_isDisposing)
        {
            HelperHandleLog("Already disposed!");
            return;
        }

        HelperHandleLog("Disposing...");
        _isDisposing = true;

        // Free client
        if (IsConnected)
        {
            try
            {
                await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disposing", CancellationToken.None);
            }
            catch { }
        }

        _monitorTimer.Dispose();
        _tokenSource?.Cancel();

        await Task.Delay(100).ContinueWith(_ =>
        {
            _joinQueue.Dispose();
            _tokenSource?.Dispose();
            _client?.Dispose();

            HelperHandleLog("Disposed!");
        });
    }

    [GeneratedRegex("^[A-Za-z0-9_]+$", RegexOptions.Compiled)]
    private static partial Regex UsernameRegex();
}