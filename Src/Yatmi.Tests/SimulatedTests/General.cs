using Yatmi.Enum;

namespace Yatmi.Tests.SimulatedTests;

public class General : TestBase
{
    [Test]
    public void StateTests()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_client.IsAnonymous, Is.True, "IsAnonymous");
            Assert.That(_client.IsConnected, Is.False, "IsConnected");
        });
    }

    [Test]
    public async Task OnLog()
    {
        var flag = new Flag();

        _client.OnLog += (s, e) =>
        {
            Assert.That(e.Message, Is.EqualTo("Sending 3 simulated messages..."));
            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            ":tmi.twitch.tv CAP * ACK :twitch.tv/membership",
            ":tmi.twitch.tv CAP * ACK :twitch.tv/commands",
            ":tmi.twitch.tv CAP * ACK :twitch.tv/tags"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnRawIrcMessage()
    {
        var flag = new Flag();

        var msg = $"{KnownCommands.PING} :tmi.twitch.tv";

        _client.OnRawIrcMessage += (s, e) =>
        {
            Assert.That(e.RawIrcMessage, Is.EqualTo(msg));
            flag.Set();
        };

        await _client.SimulateMessagesAsync(msg);

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnUnknownMessage()
    {
        var flag = new Flag();

        _client.OnUnknownMessage += (s, e) =>
        {
            Assert.That(e.RawIrcMessage, Does.Contain("CMD404"));
            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $":{_client.Username}!{_client.Username}@{_client.Username}.tmi.twitch.tv CMD404 :Beep boop"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnParsedIrcMessage()
    {
        var flag = new Flag();

        _client.OnParsedIrcMessage += (s, e) =>
        {
            Assert.That(e.ParsedIrcMessage.Code, Is.EqualTo(KnownCommands.PING));
            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"{KnownCommands.PING} :tmi.twitch.tv"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    [TestCase(KnownMessageIds.HOST_ON)]
    [TestCase(KnownMessageIds.SUBS_OFF)]
    public async Task OnNotice(string noticeType)
    {
        var flag = new Flag();

        _client.OnNotice += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.NoticeType, Is.EqualTo(noticeType), "NoticeType");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@msg-id={noticeType} :tmi.twitch.tv {KnownCommands.NOTICE} #{DUMMY_CHANNEL} :Now hosting twitch."
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task IncludeParsed_Yes()
    {
        var flag = new Flag();

        _client.OnNotice += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.NoticeType, Is.EqualTo(KnownMessageIds.HOST_ON), "NoticeType");
                Assert.That(e.ParsedIrcMessage, Is.Not.Null);
                Assert.That(e.ParsedIrcMessage.Code, Is.EqualTo(KnownCommands.NOTICE));
                Assert.That(e.ParsedIrcMessage.Channel, Is.EqualTo(DUMMY_CHANNEL));
            });

            flag.Set();
        };

        _client.IncludeParsedIrcMessagesInEvents = true;

        await _client.SimulateMessagesAsync(
            $"@msg-id={KnownMessageIds.HOST_ON} :tmi.twitch.tv {KnownCommands.NOTICE} #{DUMMY_CHANNEL} :Now hosting twitch."
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task IncludeParsed_No()
    {
        var flag = new Flag();

        _client.OnNotice += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.NoticeType, Is.EqualTo(KnownMessageIds.HOST_ON), "NoticeType");
                Assert.That(e.ParsedIrcMessage, Is.Null);
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@msg-id={KnownMessageIds.HOST_ON} :tmi.twitch.tv {KnownCommands.NOTICE} #{DUMMY_CHANNEL} :Now hosting twitch."
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task IncludeParsedWithRaw_Yes()
    {
        var flag = new Flag();
        var rawIrcMessage = $"@msg-id={KnownMessageIds.HOST_ON} :tmi.twitch.tv {KnownCommands.NOTICE} #{DUMMY_CHANNEL} :Now hosting twitch.";

        _client.OnNotice += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.NoticeType, Is.EqualTo(KnownMessageIds.HOST_ON), "NoticeType");
                Assert.That(e.ParsedIrcMessage, Is.Not.Null);
                Assert.That(e.ParsedIrcMessage.Code, Is.EqualTo(KnownCommands.NOTICE));
                Assert.That(e.ParsedIrcMessage.Channel, Is.EqualTo(DUMMY_CHANNEL));
                Assert.That(e.ParsedIrcMessage.RawIrcMessage, Is.EqualTo(rawIrcMessage));
            });

            flag.Set();
        };

        _client.IncludeParsedIrcMessagesInEvents = true;
        _client.KeepRawIrcMessageAfterParsed = true;

        await _client.SimulateMessagesAsync(rawIrcMessage);

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task IncludeParsedWithRaw_No()
    {
        var flag = new Flag();

        _client.OnNotice += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.NoticeType, Is.EqualTo(KnownMessageIds.HOST_ON), "NoticeType");
                Assert.That(e.ParsedIrcMessage, Is.Not.Null);
                Assert.That(e.ParsedIrcMessage.RawIrcMessage, Is.Null);
            });

            flag.Set();
        };

        _client.IncludeParsedIrcMessagesInEvents = true;

        await _client.SimulateMessagesAsync(
            $"@msg-id={KnownMessageIds.HOST_ON} :tmi.twitch.tv {KnownCommands.NOTICE} #{DUMMY_CHANNEL} :Now hosting twitch."
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnPing()
    {
        var flag = new Flag();

        _client.OnPing += (s, e) => flag.Set();

        await _client.SimulateMessagesAsync(
            $"{KnownCommands.PING} :tmi.twitch.tv"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnUserJoinedChannel()
    {
        var flag = new Flag();

        _client.OnUserJoinedChannel += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.JoinedChannel, Is.EqualTo(DUMMY_CHANNEL), "JoinedChannel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $":{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.JOIN} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnUserPartedChannel()
    {
        var flag = new Flag();

        _client.OnUserPartedChannel += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.PartedChannel, Is.EqualTo(DUMMY_CHANNEL), "PartedChannel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $":{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PART} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChatMessage_Normal_First_Me()
    {
        var flag = new Flag();

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");

                Assert.That(e.IsVip, Is.False, "IsVip");
                Assert.That(e.IsSubscriber, Is.False, "IsSubscriber");
                Assert.That(e.IsFounder, Is.False, "IsFounder");
                Assert.That(e.IsModerator, Is.False, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.False, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.True, "IsFirstMessage");
                Assert.That(e.IsMe, Is.True, "IsMe");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            "@badge-info=;badges=;color=#121212;" +
            $"display-name={DUMMY_USERNAME};emotes=;first-msg=1;flags=;id={GUID};mod=0;" +
            "room-id=00000000;subscriber=1;tmi-sent-ts=1656969696969;turbo=0;user-id=00000000;" +
            $"user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :\u0001ACTION Hello World!\u0001"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    [TestCase(KnownMessageIds.HIGHLIGHTED_MESSAGE, MessageTypes.Highlight)]
    [TestCase(KnownMessageIds.ANNOUNCEMENT, MessageTypes.Announcement)]
    [TestCase(KnownMessageIds.USER_INTRO, MessageTypes.UserIntroduction)]
    [TestCase("", MessageTypes.CustomReward)]
    public async Task OnChatMessage_MessageTypes(string messageID, MessageTypes expectedType)
    {
        var flag = new Flag();

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");

                Assert.That(e.IsVip, Is.False, "IsVip");
                Assert.That(e.IsSubscriber, Is.False, "IsSubscriber");
                Assert.That(e.IsFounder, Is.False, "IsFounder");
                Assert.That(e.IsModerator, Is.False, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.False, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.False, "IsFirstMessage");
                Assert.That(e.IsMe, Is.False, "IsMe");
                Assert.That(e.MessageType, Is.EqualTo(expectedType), "MessageType");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            "@badge-info=;badges=;color=#121212;" +
            $"display-name={DUMMY_USERNAME};emotes=;first-msg=0;flags=;id={GUID};msg-id={messageID};mod=0;" + (messageID?.Length == 0 ? "custom-reward-id=1234;" : "") +
            "room-id=00000000;subscriber=1;tmi-sent-ts=1656969696969;turbo=0;user-id=00000000;" +
            $"user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChatMessage_VIP_Sub()
    {
        var flag = new Flag();

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");

                Assert.That(e.IsVip, Is.True, "IsVip");
                Assert.That(e.IsSubscriber, Is.True, "IsSubscriber");
                Assert.That(e.IsFounder, Is.False, "IsFounder");
                Assert.That(e.IsModerator, Is.False, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.False, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.False, "IsFirstMessage");
                Assert.That(e.IsMe, Is.False, "IsMe");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            "@badge-info=subscriber/1;badges=vip/1,subscriber/1;color=#121212;" +
            $"display-name={DUMMY_USERNAME};emotes=;first-msg=0;flags=;id={GUID};mod=0;" +
            "room-id=00000000;subscriber=1;tmi-sent-ts=1656969696969;turbo=0;user-id=00000000;" +
            $"user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChatMessage_PaidMessage()
    {
        var flag = new Flag();

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");

                Assert.That(e.IsVip, Is.False, "IsVip");
                Assert.That(e.IsSubscriber, Is.True, "IsSubscriber");
                Assert.That(e.IsFounder, Is.False, "IsFounder");
                Assert.That(e.IsModerator, Is.False, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.False, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.False, "IsFirstMessage");
                Assert.That(e.IsMe, Is.False, "IsMe");

                Assert.That(e.PaidChat, Is.Not.Null, "PaidChat");
                Assert.That(e.PaidChat.Amount, Is.EqualTo(10), "PaidChat.Amount");
                Assert.That(e.PaidChat.CanonicalAmount, Is.EqualTo(10), "PaidChat.CanonicalAmount");
                Assert.That(e.PaidChat.Exponent, Is.EqualTo(2), "PaidChat.Exponent");
                Assert.That(e.PaidChat.IsPaidSystemMessage, Is.False, "PaidChat.IsPaidSystemMessage");
                Assert.That(e.PaidChat.Level, Is.EqualTo("ONE"), "PaidChat.Level");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            "@badge-info=subscriber/1;badges=subscriber/1;color=#121212;" +
            $"display-name={DUMMY_USERNAME};emotes=;first-msg=0;flags=;id={GUID};mod=0;" +
            "pinned-chat-paid-amount=1000;pinned-chat-paid-canonical-amount=1000;pinned-chat-paid-currency=USD;pinned-chat-paid-exponent=2;pinned-chat-paid-is-system-message=0;pinned-chat-paid-level=ONE;" +
            "room-id=00000000;subscriber=1;tmi-sent-ts=1656969696969;turbo=0;user-id=00000000;" +
            $"user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChatMessage_VIP_Founder()
    {
        var flag = new Flag();

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");

                Assert.That(e.IsVip, Is.True, "IsVip");
                Assert.That(e.IsSubscriber, Is.False, "IsSubscriber");
                Assert.That(e.IsFounder, Is.True, "IsFounder");
                Assert.That(e.IsModerator, Is.False, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.False, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.False, "IsFirstMessage");
                Assert.That(e.IsMe, Is.False, "IsMe");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            "@badge-info=founder/1;badges=vip/1,founder/0;color=#121212;" +
            $"display-name={DUMMY_USERNAME};emotes=;first-msg=0;flags=;id={GUID};mod=0;" +
            "room-id=00000000;subscriber=1;tmi-sent-ts=1656969696969;turbo=0;user-id=00000000;" +
            $"user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChatMessage_Mod_Sub()
    {
        var flag = new Flag();

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");

                Assert.That(e.IsVip, Is.False, "IsVip");
                Assert.That(e.IsSubscriber, Is.True, "IsSubscriber");
                Assert.That(e.IsFounder, Is.False, "IsFounder");
                Assert.That(e.IsModerator, Is.True, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.False, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.False, "IsFirstMessage");
                Assert.That(e.IsMe, Is.False, "IsMe");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            "@badge-info=subscriber/1;badges=moderator/1,subscriber/1;color=#121212;" +
            $"display-name={DUMMY_USERNAME};emotes=;first-msg=0;flags=;id={GUID};mod=1;" +
            "room-id=00000000;subscriber=1;tmi-sent-ts=1656969696969;turbo=0;user-id=00000000;" +
            $"user-type=mod :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChatMessage_Mod_Founder()
    {
        var flag = new Flag();

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");

                Assert.That(e.IsVip, Is.False, "IsVip");
                Assert.That(e.IsSubscriber, Is.False, "IsSubscriber");
                Assert.That(e.IsFounder, Is.True, "IsFounder");
                Assert.That(e.IsModerator, Is.True, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.False, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.False, "IsFirstMessage");
                Assert.That(e.IsMe, Is.False, "IsMe");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            "@badge-info=founder/1;badges=moderator/1,founder/0;color=#121212;" +
            $"display-name={DUMMY_USERNAME};emotes=;first-msg=0;flags=;id={GUID};mod=1;" +
            "room-id=00000000;subscriber=1;tmi-sent-ts=1656969696969;turbo=0;user-id=00000000;" +
            $"user-type=mod :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChatMessage_Broadcaster()
    {
        var flag = new Flag();

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");

                Assert.That(e.IsVip, Is.False, "IsVip");
                Assert.That(e.IsSubscriber, Is.True, "IsSubscriber");
                Assert.That(e.IsFounder, Is.False, "IsFounder");
                Assert.That(e.IsModerator, Is.False, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.True, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.False, "IsFirstMessage");
                Assert.That(e.IsMe, Is.False, "IsMe");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            "@badge-info=founder/1;badges=broadcaster/1,subscriber/1;color=#121212;" +
            $"display-name={DUMMY_USERNAME};emotes=;first-msg=0;flags=;id={GUID};mod=0;" +
            "room-id=00000000;subscriber=1;tmi-sent-ts=1656969696969;turbo=0;user-id=00000000;" +
            $"user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChatMessage_Staff()
    {
        var flag = new Flag();

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");

                Assert.That(e.IsStaff, Is.True, "IsStaff");
                Assert.That(e.IsVip, Is.False, "IsVip");
                Assert.That(e.IsSubscriber, Is.False, "IsSubscriber");
                Assert.That(e.IsFounder, Is.False, "IsFounder");
                Assert.That(e.IsModerator, Is.False, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.False, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.False, "IsFirstMessage");
                Assert.That(e.IsMe, Is.False, "IsMe");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            "@badge-info=subscriber/1;badges=staff/1;color=#121212;" +
            $"display-name={DUMMY_USERNAME};emotes=;first-msg=0;flags=;id={GUID};mod=0;" +
            "room-id=00000000;subscriber=0;tmi-sent-ts=1656969696969;turbo=0;user-id=00000000;" +
            $"user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChatMessage_EmoteParse_Yes()
    {
        var flag = new Flag(4);

        var counter = 0;
        var expected = new[]{
            "Hello <img alt=\"HeyGuys\" src=\"https://static-cdn.jtvnw.net/emoticons/v2/30259/default/dark/1.0\" />",
            "Hello <img alt=\"HeyGuys\" src=\"https://static-cdn.jtvnw.net/emoticons/v2/30259/default/dark/1.0\" /> !",
            "<img alt=\"HeyGuys\" src=\"https://static-cdn.jtvnw.net/emoticons/v2/30259/default/dark/1.0\" /> Hello!",
            "<img alt=\"HeyGuys\" src=\"https://static-cdn.jtvnw.net/emoticons/v2/30259/default/dark/1.0\" />"
        };

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");

                Assert.That(e.Emotes.RenderMessageAsHtml(e.Message), Is.EqualTo(expected[counter++]), "HTML");
            });

            flag.Set();
        };

        _client.ParseEmotesInMessages = true;

        await _client.SimulateMessagesAsync(
            $"@badge-info=;badges=;client-nonce=00000000000000000000000000000000;color=;display-name={DUMMY_USERNAME};emotes=30259:6-12;first-msg=0;flags=;id={GUID};mod=0;returning-chatter=0;room-id=00000000;subscriber=0;tmi-sent-ts=1666969696969;turbo=0;user-id=000000000;user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello HeyGuys",
            $"@badge-info=;badges=;client-nonce=00000000000000000000000000000000;color=;display-name={DUMMY_USERNAME};emotes=30259:6-12;first-msg=0;flags=;id={GUID};mod=0;returning-chatter=0;room-id=00000000;subscriber=0;tmi-sent-ts=1666969696969;turbo=0;user-id=000000000;user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello HeyGuys !",
            $"@badge-info=;badges=;client-nonce=00000000000000000000000000000000;color=;display-name={DUMMY_USERNAME};emotes=30259:0-6;first-msg=0;flags=;id={GUID};mod=0;returning-chatter=0;room-id=00000000;subscriber=0;tmi-sent-ts=1666969696969;turbo=0;user-id=000000000;user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :HeyGuys Hello!",
            $"@badge-info=;badges=;client-nonce=00000000000000000000000000000000;color=;display-name={DUMMY_USERNAME};emote-only=1;emotes=30259:0-6;first-msg=0;flags=;id={GUID};mod=0;returning-chatter=0;room-id=00000000;subscriber=0;tmi-sent-ts=1666969696969;turbo=0;user-id=000000000;user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :HeyGuys"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChatMessage_EmoteParse_No()
    {
        var flag = new Flag(4);

        var counter = 0;
        var expected = new[]{
            "Hello HeyGuys",
            "Hello HeyGuys !",
            "HeyGuys Hello!",
            "HeyGuys"
        };

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");

                Assert.That(e.Emotes.RenderMessageAsHtml(e.Message), Is.EqualTo(expected[counter++]), "HTML");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=;badges=;client-nonce=00000000000000000000000000000000;color=;display-name={DUMMY_USERNAME};emotes=30259:6-12;first-msg=0;flags=;id={GUID};mod=0;returning-chatter=0;room-id=00000000;subscriber=0;tmi-sent-ts=1666969696969;turbo=0;user-id=000000000;user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello HeyGuys",
            $"@badge-info=;badges=;client-nonce=00000000000000000000000000000000;color=;display-name={DUMMY_USERNAME};emotes=30259:6-12;first-msg=0;flags=;id={GUID};mod=0;returning-chatter=0;room-id=00000000;subscriber=0;tmi-sent-ts=1666969696969;turbo=0;user-id=000000000;user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Hello HeyGuys !",
            $"@badge-info=;badges=;client-nonce=00000000000000000000000000000000;color=;display-name={DUMMY_USERNAME};emotes=30259:0-6;first-msg=0;flags=;id={GUID};mod=0;returning-chatter=0;room-id=00000000;subscriber=0;tmi-sent-ts=1666969696969;turbo=0;user-id=000000000;user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :HeyGuys Hello!",
            $"@badge-info=;badges=;client-nonce=00000000000000000000000000000000;color=;display-name={DUMMY_USERNAME};emote-only=1;emotes=30259:0-6;first-msg=0;flags=;id={GUID};mod=0;returning-chatter=0;room-id=00000000;subscriber=0;tmi-sent-ts=1666969696969;turbo=0;user-id=000000000;user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :HeyGuys"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(10000)]
    [TestCase(100000)]
    public async Task OnChatMessage_Cheer(int cheer)
    {
        var flag = new Flag(2);

        _client.OnChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo($"Cheer{cheer} Hello World!"), "Message");

                Assert.That(e.IsVip, Is.False, "IsVip");
                Assert.That(e.IsSubscriber, Is.False, "IsSubscriber");
                Assert.That(e.IsFounder, Is.False, "IsFounder");
                Assert.That(e.IsModerator, Is.False, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.False, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.False, "IsFirstMessage");
                Assert.That(e.IsMe, Is.False, "IsMe");
                Assert.That(e.Bits, Is.EqualTo(cheer), "Bits");
            });

            flag.Set();
        };

        _client.OnBitsChatMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Message, Is.EqualTo($"Cheer{cheer} Hello World!"), "Message");

                Assert.That(e.IsVip, Is.False, "IsVip");
                Assert.That(e.IsSubscriber, Is.False, "IsSubscriber");
                Assert.That(e.IsFounder, Is.False, "IsFounder");
                Assert.That(e.IsModerator, Is.False, "IsModerator");
                Assert.That(e.IsBroadcaster, Is.False, "IsBroadcaster");
                Assert.That(e.IsFirstMessage, Is.False, "IsFirstMessage");
                Assert.That(e.IsMe, Is.False, "IsMe");
                Assert.That(e.Bits, Is.EqualTo(cheer), "Bits");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=;badges=bits-leader/1;bits={cheer};color=#121212;" +
            $"display-name={DUMMY_USERNAME};emotes=;first-msg=0;flags=;id={GUID};mod=0;" +
            "room-id=00000000;subscriber=1;tmi-sent-ts=1656969696969;turbo=0;user-id=00000000;" +
            $"user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {KnownCommands.PRIVMSG} #{DUMMY_CHANNEL} :Cheer{cheer} Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "All event was not raised!");
    }

    [Test]
    public async Task OnWhisperMessage()
    {
        var flag = new Flag();

        _client.OnWhisperMessage += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badges=twitchconEU2022/1;color=#121212;display-name={DUMMY_USERNAME};emotes=;message-id=00;thread-id=00000000_000000000;" +
            $"turbo=0;user-id=00000000;user-type= :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv " +
            $"{KnownCommands.WHISPER} {_client.Username} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "All event was not raised!");
    }

    [Test]
    public async Task OnRaided()
    {
        var flag = new Flag();

        _client.OnRaided += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Raider, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Viewers, Is.EqualTo(10), "Viewers");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=;badges=;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={GUID};login={DUMMY_USERNAME};mod=0;msg-id=raid;msg-param-displayName={DUMMY_USERNAME};" +
            $"msg-param-login={DUMMY_USERNAME};msg-param-profileImageURL=https://static-cdn.jtvnw.net/jtv_user_pictures/{GUID}-profile_image-70x70.png;" +
            @$"msg-param-viewerCount=10;room-id=00000000;subscriber=0;system-msg=10\sraiders\sfrom\s{DUMMY_USERNAME}\shave\sjoined!;" +
            $"tmi-sent-ts=1656969696969;user-id=00000006;user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "All event was not raised!");
    }

    [Test]
    public async Task OnRaidCancelled()
    {
        var flag = new Flag();

        _client.OnRaidCancelled += (s, e) =>
        {
            Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/1;badges=subscriber/1;color=#121212;display-name={DUMMY_USERNAME};" +
            $"emotes=;flags=;id={GUID};login={DUMMY_USERNAME};mod=0;msg-id=unraid;room-id=00000000;subscriber=1;" +
            @"system-msg=The\sraid\shas\sbeen\scanceled.;tmi-sent-ts=1660407082773;user-id=00000006;" +
            $"user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "All event was not raised!");
    }

    [Test]
    public async Task OnHosted()
    {
        var flag = new Flag(2);

        _client.OnHosted += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Hosted, Is.EqualTo(DUMMY_USERNAME), "Hosted");
                Assert.That(e.Viewers, Is.EqualTo(420), "Viewers");
            });

            flag.Set();
        };

        _client.OnNotice += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.NoticeType, Is.EqualTo(KnownMessageIds.HOST_ON), "NoticeType");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $":tmi.twitch.tv {KnownCommands.HOSTTARGET} #{DUMMY_CHANNEL} :{DUMMY_USERNAME} 420",
            $"@msg-id={KnownMessageIds.HOST_ON} :tmi.twitch.tv {KnownCommands.NOTICE} #{DUMMY_CHANNEL} :Now hosting {DUMMY_USERNAME}."
        );

        Assert.That(flag.Wait(), Is.True, "All event was not raised!");
    }

    [Test]
    public async Task OnNamesList()
    {
        var flag = new Flag();

        var dummyNames = Enumerable.Range(1, 10).Select(a => $"{DUMMY_USERNAME}_{a}").ToArray();

        _client.OnNamesList += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Usernames, Is.EqualTo(dummyNames), "Usernames");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $":{_client.Username}!{_client.Username}@{_client.Username}.tmi.twitch.tv JOIN #{DUMMY_CHANNEL}",
            $"@emote-only=0;followers-only=-1;r9k=0;room-id=00000000;slow=0;subs-only=0 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}",
            $":{_client.Username}.tmi.twitch.tv {KnownCommands.NAMES_LIST} {_client.Username} = #{DUMMY_CHANNEL} :{string.Join(" ", dummyNames)}",
            $":{_client.Username}.tmi.twitch.tv {KnownCommands.NAMES_LIST} {_client.Username} = #{DUMMY_CHANNEL} :{_client.Username}",
            $":{_client.Username}.tmi.twitch.tv {KnownCommands.END_OF_NAMES_LIST} {_client.Username} #{DUMMY_CHANNEL} :End of /NAMES list"
        );

        Assert.That(flag.Wait(), Is.True, "All event was not raised!");
    }

    [Test]
    public async Task OnBitsBadge()
    {
        var flag = new Flag();

        _client.OnBitsBadge += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.AutoSystemMessage, Is.EqualTo($"{DUMMY_USERNAME} just unlocked the 50000 badge!"), "AutoSystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/1;badges=subscriber/1,bits/50000;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={GUID};login={DUMMY_USERNAME};mod=0;msg-id=bitsbadgetier;msg-param-threshold=50000;room-id=00000000;subscriber=1;" +
            @"system-msg=bits\sbadge\stier\snotification;tmi-sent-ts=1656969696969;user-id=000000000;" +
            $"user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "All event was not raised!");
    }

    [Test]
    public async Task OnChannelJoined()
    {
        var flag = new Flag();

        _client.OnChannelJoined += (s, e) =>
        {
            Assert.That(e.JoinedChannel, Is.EqualTo(DUMMY_CHANNEL), "JoinedChannel");
            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $":{_client.Username}!{_client.Username}@{_client.Username}.tmi.twitch.tv {KnownCommands.JOIN} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnChannelParted()
    {
        var flag = new Flag();

        _client.OnChannelParted += (s, e) =>
        {
            Assert.That(e.PartedChannel, Is.EqualTo(DUMMY_CHANNEL), "PartedChannel");
            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $":{_client.Username}!{_client.Username}@{_client.Username}.tmi.twitch.tv {KnownCommands.PART} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task CanGetRoomStateAllOff()
    {
        var flag = new Flag();

        _client.OnRoomState += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.EmoteOnly, Is.False, "EmoteOnly");
                Assert.That(e.FollowersOnly, Is.False, "FollowersOnly");
                Assert.That(e.FollowersTime, Is.EqualTo(TimeSpan.FromSeconds(0)), "FollowersTime");
                Assert.That(e.SlowMode, Is.EqualTo(TimeSpan.FromSeconds(0)), "SlowMode");
                Assert.That(e.SubsOnly, Is.False, "SubsOnly");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@emote-only=0;followers-only=-1;r9k=0;room-id=00000000;slow=0;subs-only=0 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task CanGetRoomStateAllOn()
    {
        var flag = new Flag();

        _client.OnRoomState += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.EmoteOnly, Is.True, "EmoteOnly");
                Assert.That(e.FollowersOnly, Is.True, "FollowersOnly");
                Assert.That(e.FollowersTime, Is.EqualTo(TimeSpan.FromMinutes(5)), "FollowersTime");
                Assert.That(e.SlowMode, Is.EqualTo(TimeSpan.FromSeconds(5)), "SlowMode");
                Assert.That(e.SubsOnly, Is.True, "SubsOnly");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@emote-only=1;followers-only=5;r9k=0;room-id=00000000;slow=5;subs-only=1 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task CanGetRoomStateIndividualOff()
    {
        var flag = new Flag(4);

        _client.OnEmoteOnlyState += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.IsActive, Is.False, "EmoteOnly");
            });

            flag.Set();
        };

        _client.OnFollowersOnlyState += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.FollowersOnly, Is.False, "FollowersOnly");
                Assert.That(e.FollowersTime, Is.EqualTo(TimeSpan.FromSeconds(0)), "FollowersTime");
            });

            flag.Set();
        };

        _client.OnSlowModeState += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.SlowMode, Is.EqualTo(TimeSpan.FromSeconds(0)), "SlowMode");
            });

            flag.Set();
        };

        _client.OnSubscribersOnlyState += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.IsActive, Is.False, "SubsOnly");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@emote-only=0;room-id=00000000 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}",
            $"@followers-only=-1;room-id=00000000 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}",
            $"@slow=0;room-id=00000000 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}",
            $"@subs-only=0;room-id=00000000 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "All event was not raised!");
    }

    [Test]
    public async Task CanGetRoomStateIndividualOn()
    {
        var flag = new Flag(4);

        _client.OnEmoteOnlyState += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.IsActive, Is.True, "EmoteOnly");
            });

            flag.Set();
        };

        _client.OnFollowersOnlyState += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.FollowersOnly, Is.True, "FollowersOnly");
                Assert.That(e.FollowersTime, Is.EqualTo(TimeSpan.FromMinutes(5)), "FollowersTime");
            });

            flag.Set();
        };

        _client.OnSlowModeState += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.SlowMode, Is.EqualTo(TimeSpan.FromSeconds(5)), "SlowMode");
            });

            flag.Set();
        };

        _client.OnSubscribersOnlyState += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.IsActive, Is.True, "SubsOnly");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@emote-only=1;room-id=00000000 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}",
            $"@followers-only=5;room-id=00000000 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}",
            $"@slow=5;room-id=00000000 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}",
            $"@subs-only=1;room-id=00000000 :tmi.twitch.tv {KnownCommands.ROOMSTATE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "All event was not raised!");
    }
}