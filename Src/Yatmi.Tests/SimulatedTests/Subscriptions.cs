using Yatmi.Enum;

namespace Yatmi.Tests.SimulatedTests;

public class Subscriptions : TestBase
{
    [Test]
    [TestCase("1", SubPlanTypes.Tier1, "1000")]
    [TestCase("2", SubPlanTypes.Tier2, "2000")]
    [TestCase("3", SubPlanTypes.Tier3, "3000")]
    public async Task OnSubscribe(string tierLevel, SubPlanTypes subPlan, int ircCode)
    {
        var flag = new Flag();

        _client.OnSubscribe += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.Months, Is.EqualTo(1), "Months");
                Assert.That(e.SubPlanType, Is.EqualTo(subPlan), "SubPlanType");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} subscribed at Tier {tierLevel}."), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/0;badges=subscriber/0;color=;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=sub;msg-param-cumulative-months=1;" +
            "msg-param-months=0;msg-param-multimonth-duration=1;msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;" +
            $@"msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};msg-param-was-gifted=false;" +
            $@"room-id=000000000;subscriber=1;system-msg={DUMMY_USERNAME}\ssubscribed\sat\sTier\s{tierLevel}.;tmi-sent-ts=1654696969696;" +
            $"user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnSubscribePrime()
    {
        var flag = new Flag();

        _client.OnSubscribe += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.Months, Is.EqualTo(1), "Months");
                Assert.That(e.SubPlanType, Is.EqualTo(SubPlanTypes.Prime), "SubPlanType");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} subscribed with Prime."), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/0;badges=subscriber/0;color=;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=sub;msg-param-cumulative-months=1;msg-param-goal-contribution-type=SUB_POINTS;" +
            @"msg-param-goal-current-contributions=1211;msg-param-goal-description=I\shave\sno\sidea;msg-param-goal-target-contributions=1200;" +
            "msg-param-goal-user-contributions=1;msg-param-months=0;msg-param-multimonth-duration=1;msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;" +
            @"msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan=Prime;msg-param-was-gifted=false;room-id=00000000;subscriber=1;" +
            $@"system-msg={DUMMY_USERNAME}\ssubscribed\swith\sPrime.;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    [TestCase("1", SubPlanTypes.Tier1, "1000")]
    [TestCase("2", SubPlanTypes.Tier2, "2000")]
    [TestCase("3", SubPlanTypes.Tier3, "3000")]
    public async Task OnResubscribe(string tierLevel, SubPlanTypes subPlan, int ircCode)
    {
        var flag = new Flag();

        _client.OnResubscribe += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.Months, Is.EqualTo(16), "Months");
                Assert.That(e.SubPlanType, Is.EqualTo(subPlan), "SubPlanType");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} subscribed at Tier {tierLevel}. They've subscribed for 16 months!"), "SystemMessage");
                Assert.That(e.Message, Is.EqualTo("Hello World!"), "Message");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/16;badges=subscriber/12;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=resub;msg-param-cumulative-months=16;msg-param-months=0;msg-param-multimonth-duration=0;" +
            $"msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;msg-param-sub-plan-name=Channel\\sSubscription\\sPOG;msg-param-sub-plan={ircCode};msg-param-was-gifted=false;" +
            $@"room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\ssubscribed\sat\sTier\s{tierLevel}.\sThey've\ssubscribed\sfor\s16\smonths!;tmi-sent-ts=1654696969696;" +
            $"user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnResubscribePrime()
    {
        var flag = new Flag();

        _client.OnResubscribe += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.Months, Is.EqualTo(23), "Months");
                Assert.That(e.SubPlanType, Is.EqualTo(SubPlanTypes.Prime), "SubPlanType");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} subscribed with Prime. They've subscribed for 23 months!"), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/23;badges=subscriber/18;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=resub;msg-param-cumulative-months=23;msg-param-months=0;msg-param-multimonth-duration=0;" +
            "msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;msg-param-sub-plan-name=Channel\\sSubscription\\sPOG;msg-param-sub-plan=Prime;" +
            $@"msg-param-was-gifted=false;room-id=000000000;subscriber=1;system-msg={DUMMY_USERNAME}\ssubscribed\swith\sPrime.\sThey've\ssubscribed\sfor\s23\smonths!;" +
            $"tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL} :Hello World!"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    [TestCase("1", SubPlanTypes.Tier1, "1000")]
    [TestCase("2", SubPlanTypes.Tier2, "2000")]
    [TestCase("3", SubPlanTypes.Tier3, "3000")]
    public async Task OnGiftSubscription(string tierLevel, SubPlanTypes subPlan, int ircCode)
    {
        var flag = new Flag();

        _client.OnGiftSubscription += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.RecipientUsername, Is.EqualTo($"{DUMMY_USERNAME}_Foobar"), "RecipientUsername");
                Assert.That(e.RecipientUserID, Is.EqualTo($"1{DUMMY_USER_ID}"), "RecipientUserID");
                Assert.That(e.SubPlanType, Is.EqualTo(subPlan), "SubPlanType");
                Assert.That(e.TotalSubsGifted, Is.EqualTo(50), "TotalSubsGifted");
                Assert.That(e.SubGiftType, Is.EqualTo(SubGiftTypes.PersonalGift), "SubGiftType");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} gifted a Tier {tierLevel} sub to {DUMMY_USERNAME}_Foobar! They have given 50 Gift Subs in the channel!"), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/12;badges=subscriber/12;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};" +
            @$"login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=1;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;" +
            $"msg-param-recipient-display-name={DUMMY_USERNAME}_Foobar;msg-param-recipient-id=1{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_Foobar;msg-param-sender-count=50;" +
            $"msg-param-sub-plan-name=Channel\\sSubscription\\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;" +
            $@"system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_Foobar!\sThey\shave\sgiven\s50\sGift\sSubs\sin\sthe\schannel!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};" +
            $"user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    [TestCase("1", SubPlanTypes.Tier1, "1000")]
    [TestCase("2", SubPlanTypes.Tier2, "2000")]
    [TestCase("3", SubPlanTypes.Tier3, "3000")]
    public async Task OnGiftSubscription_Anonymous(string tierLevel, SubPlanTypes subPlan, int ircCode)
    {
        var flag = new Flag();

        _client.OnGiftSubscription += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(KnownLogins.AN_ANONYMOUS_GIFTER), "Username");
                Assert.That(e.UserID, Is.EqualTo(KnownLogins.AN_ANONYMOUS_GIFTER_ID), "UserID");
                Assert.That(e.RecipientUsername, Is.EqualTo(DUMMY_USERNAME), "RecipientUsername");
                Assert.That(e.SubGiftType, Is.EqualTo(SubGiftTypes.PersonalGift), "SubGiftType");
                Assert.That(e.TotalSubsGifted, Is.EqualTo(0), "TotalSubsGifted");
                Assert.That(e.SubPlanType, Is.EqualTo(subPlan), "SubPlanType");
                Assert.That(e.SystemMessage, Is.EqualTo($"An anonymous user gifted a Tier {tierLevel} sub to {DUMMY_USERNAME}! "), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};" +
            $"login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringFive;msg-param-gift-months=1;msg-param-months=11;" +
            @$"msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME};" +
            @$"msg-param-recipient-id=00000000;msg-param-recipient-user-name={DUMMY_USERNAME};msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;" +
            @$"subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    [TestCase("1", SubPlanTypes.Tier1, "1000")]
    [TestCase("2", SubPlanTypes.Tier2, "2000")]
    [TestCase("3", SubPlanTypes.Tier3, "3000")]
    public async Task OnCommunityGiftSubscription(string tierLevel, SubPlanTypes subPlan, int ircCode)
    {
        var flag = new Flag(6);
        var letter = 'A';

        _client.OnCommunityGiftSubscription += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.SubPlanType, Is.EqualTo(subPlan), "SubPlanType");
                Assert.That(e.SubscriptionsGifted, Is.EqualTo(5), "SubscriptionsGifted");
                Assert.That(e.TotalSubscriptionsGifted, Is.EqualTo(123), "TotalSubscriptionsGifted");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} is gifting 5 Tier {tierLevel} Subs to POG's community! They've gifted a total of 123 in the channel!"), "SystemMessage");
            });

            flag.Set();
        };

        _client.OnGiftSubscription += (s, e) =>
        {
            var okRecipientUsername = e.RecipientUsername.StartsWith(DUMMY_USERNAME) && e.RecipientUsername.Contains('_');
            var okRecipientUserID = e.RecipientUserID.EndsWith(DUMMY_USER_ID) && !e.RecipientUserID.Contains('_');

            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(okRecipientUsername, Is.True, "RecipientUsername");
                Assert.That(okRecipientUserID, Is.True, "RecipientUserID");
                Assert.That(e.SubPlanType, Is.EqualTo(subPlan), "SubPlanType");
                Assert.That(e.SubGiftType, Is.EqualTo(SubGiftTypes.CommunityGift), "SubGiftType");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} gifted a Tier {tierLevel} sub to {DUMMY_USERNAME}_{letter++}!"), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=submysterygift;msg-param-goal-contribution-type=SUB_POINTS;msg-param-goal-current-contributions=497;msg-param-goal-description=nice;msg-param-goal-target-contributions=699;msg-param-goal-user-contributions=5;msg-param-mass-gift-count=5;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-sender-count=123;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sis\sgifting\s5\sTier\s{tierLevel}\sSubs\sto\sPOG's\scommunity!\sThey've\sgifted\sa\stotal\sof\s123\sin\sthe\schannel!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=2;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_A;msg-param-recipient-id=1{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_A;msg-param-sender-count=0;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_A!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=5;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_B;msg-param-recipient-id=2{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_B;msg-param-sender-count=0;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_B!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=2;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_C;msg-param-recipient-id=3{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_C;msg-param-sender-count=0;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_C!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=8;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_D;msg-param-recipient-id=4{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_D;msg-param-sender-count=0;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_D!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=1;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_E;msg-param-recipient-id=5{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_E;msg-param-sender-count=0;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_E!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    [TestCase("1", SubPlanTypes.Tier1, "1000")]
    [TestCase("2", SubPlanTypes.Tier2, "2000")]
    [TestCase("3", SubPlanTypes.Tier3, "3000")]
    public async Task OnCommunityGiftSubscription_Anonymous(string tierLevel, SubPlanTypes subPlan, int ircCode)
    {
        var flag = new Flag(6);
        var letter = 'A';

        _client.OnCommunityGiftSubscription += (s, e) =>
        {

            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(KnownLogins.AN_ANONYMOUS_GIFTER), "Username");
                Assert.That(e.UserID, Is.EqualTo(KnownLogins.AN_ANONYMOUS_GIFTER_ID), "UserID");
                Assert.That(e.SubPlanType, Is.EqualTo(subPlan), "SubPlanType");
                Assert.That(e.SubscriptionsGifted, Is.EqualTo(5), "SubscriptionsGifted");
                Assert.That(e.TotalSubscriptionsGifted, Is.EqualTo(0), "TotalSubscriptionsGifted");
                Assert.That(e.SystemMessage, Is.EqualTo($"An anonymous user is gifting 5 Tier {tierLevel} Subs to POG's community!"), "SystemMessage");
            });

            flag.Set();
        };

        _client.OnGiftSubscription += (s, e) =>
        {
            var okRecipientUsername = e.RecipientUsername.StartsWith(DUMMY_USERNAME) && e.RecipientUsername.Contains('_');
            var okRecipientUserID = e.RecipientUserID.EndsWith(DUMMY_USER_ID) && !e.RecipientUserID.Contains('_');

            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(KnownLogins.AN_ANONYMOUS_GIFTER), "Username");
                Assert.That(e.UserID, Is.EqualTo(KnownLogins.AN_ANONYMOUS_GIFTER_ID), "UserID");
                Assert.That(okRecipientUsername, Is.True, "RecipientUsername");
                Assert.That(okRecipientUserID, Is.True, "RecipientUserID");
                Assert.That(e.SubPlanType, Is.EqualTo(subPlan), "SubPlanType");
                Assert.That(e.SubGiftType, Is.EqualTo(SubGiftTypes.CommunityGift), "SubGiftType");
                Assert.That(e.SystemMessage, Is.EqualTo($"An anonymous user gifted a Tier {tierLevel} sub to {DUMMY_USERNAME}_{letter++}! "), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=submysterygift;msg-param-mass-gift-count=5;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sis\sgifting\s5\sTier\s{tierLevel}\sSubs\sto\sPOG's\scommunity!;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringFive;msg-param-gift-months=1;msg-param-months=1;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_A;msg-param-recipient-id=1{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_A;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_A!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringThree;msg-param-gift-months=1;msg-param-months=5;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_B;msg-param-recipient-id=2{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_B;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_B!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringFour;msg-param-gift-months=1;msg-param-months=1;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_C;msg-param-recipient-id=3{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_C;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_C!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringTwo;msg-param-gift-months=1;msg-param-months=6;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_D;msg-param-recipient-id=4{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_D;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_D!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringFive;msg-param-gift-months=1;msg-param-months=9;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_E;msg-param-recipient-id=5{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_E;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_E!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnContinuingGiftSubscription()
    {
        var flag = new Flag();

        _client.OnContinuingGiftSubscription += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.GifterUsername, Is.EqualTo($"{DUMMY_USERNAME}_Foo"), "GifterUsername");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} is continuing the Gift Sub they got from {DUMMY_USERNAME}_Foo!"), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/5;badges=subscriber/3,bits/100;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=giftpaidupgrade;msg-param-sender-login={DUMMY_USERNAME}_Foo;msg-param-sender-name={DUMMY_USERNAME}_Foo;" +
            @$"room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sis\scontinuing\sthe\sGift\sSub\sthey\sgot\sfrom\s{DUMMY_USERNAME}_Foo!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};" +
            $"user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnContinuingGiftSubscription_Anonymous()
    {
        var flag = new Flag();

        _client.OnContinuingAnonymousGiftSubscription += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} is continuing the Gift Sub they got from an anonymous user!"), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/3;badges=subscriber/3;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=anongiftpaidupgrade;room-id=00000000;subscriber=1;" +
            @$"system-msg={DUMMY_USERNAME}\sis\scontinuing\sthe\sGift\sSub\sthey\sgot\sfrom\san\sanonymous\suser!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};" +
            $"user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    [TestCase("1", SubPlanTypes.Tier1, "1000")]
    [TestCase("2", SubPlanTypes.Tier2, "2000")]
    [TestCase("3", SubPlanTypes.Tier3, "3000")]
    public async Task OnPrimeUpgradeSubscription(string tierLevel, SubPlanTypes subPlan, int ircCode)
    {
        var flag = new Flag();

        _client.OnPrimeUpgradeSubscription += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.SubPlanType, Is.EqualTo(subPlan), "SubPlanType");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME}s converted from a Prime sub to a Tier {tierLevel} sub!"), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/1;badges=subscriber/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=primepaidupgrade;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;" +
            @$"system-msg={DUMMY_USERNAME}s\sconverted\sfrom\sa\sPrime\ssub\sto\sa\sTier\s{tierLevel}\ssub!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};" +
            $"user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnCommunityPayForward()
    {
        var flag = new Flag();

        _client.OnCommunityPayForward += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.GifterUsername, Is.EqualTo($"{DUMMY_USERNAME}_Foo"), "GifterUsername");
                Assert.That(e.GifterUserID, Is.EqualTo($"1{DUMMY_USER_ID}"), "GifterUserID");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} is paying forward the Gift they got from {DUMMY_USERNAME}_Foo to the community!"), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/3;badges=subscriber/3;color=;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=communitypayforward;msg-param-prior-gifter-anonymous=false;" +
            $"msg-param-prior-gifter-display-name={DUMMY_USERNAME}_Foo;msg-param-prior-gifter-id=1{DUMMY_USER_ID};msg-param-prior-gifter-user-name={DUMMY_USERNAME}_Foo;room-id=000000000;subscriber=1;" +
            @$"system-msg={DUMMY_USERNAME}\sis\spaying\sforward\sthe\sGift\sthey\sgot\sfrom\s{DUMMY_USERNAME}_Foo\sto\sthe\scommunity!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};" +
            $"user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    public async Task OnSubscriptionPayForward()
    {
        var flag = new Flag();

        _client.OnSubscriptionPayForward += (s, e) =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                Assert.That(e.Username, Is.EqualTo(DUMMY_USERNAME), "Username");
                Assert.That(e.UserID, Is.EqualTo(DUMMY_USER_ID), "UserID");
                Assert.That(e.GifterUsername, Is.EqualTo($"{DUMMY_USERNAME}_Foo"), "GifterUsername");
                Assert.That(e.GifterUserID, Is.EqualTo($"1{DUMMY_USER_ID}"), "GifterUserID");
                Assert.That(e.ReceiverUsername, Is.EqualTo($"{DUMMY_USERNAME}_Bar"), "ReceiverUsername");
                Assert.That(e.ReceiverUserID, Is.EqualTo($"2{DUMMY_USER_ID}"), "ReceiverUserID");
                Assert.That(e.SystemMessage, Is.EqualTo($"{DUMMY_USERNAME} is paying forward the Gift they got from {DUMMY_USERNAME}_Foo to {DUMMY_USERNAME}_Bar!"), "SystemMessage");
            });

            flag.Set();
        };

        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/30;badges=subscriber/24;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=standardpayforward;msg-param-prior-gifter-anonymous=false;" +
            $"msg-param-prior-gifter-display-name={DUMMY_USERNAME}_Foo;msg-param-prior-gifter-id=1{DUMMY_USER_ID};msg-param-prior-gifter-user-name={DUMMY_USERNAME}_Foo;msg-param-recipient-display-name={DUMMY_USERNAME}_Bar;" +
            @$"msg-param-recipient-id=2{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_Bar;room-id=000000000;subscriber=1;system-msg={DUMMY_USERNAME}\sis\spaying\sforward\sthe\sGift\sthey\sgot\sfrom\s{DUMMY_USERNAME}_Foo\sto\s{DUMMY_USERNAME}_Bar!;" +
            $"tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }

    [Test]
    [TestCase("1", "1000")]
    [TestCase("2", "2000")]
    [TestCase("3", "3000")]
    public async Task OnSubscriptionCounter(string tierLevel, int ircCode)
    {
        var flag = new Flag(8);
        var counter = 0;

        var expectedUserIDs = new[]{
            DUMMY_USER_ID,
            KnownLogins.AN_ANONYMOUS_GIFTER_ID
        };

        _client.OnSubscriptionCounter += (s, e) =>
        {
            if (!expectedUserIDs.Contains(e.UserID))
            {
                throw new Exception($"UserID '{e.UserID}' was not expected!");
            }

            counter += e.SubscriptionCount;
            flag.Set();
        };

        // Subscribe
        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/0;badges=subscriber/0;color=;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=sub;msg-param-cumulative-months=1;" +
            "msg-param-months=0;msg-param-multimonth-duration=1;msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;" +
            $@"msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};msg-param-was-gifted=false;" +
            $@"room-id=000000000;subscriber=1;system-msg={DUMMY_USERNAME}\ssubscribed\sat\sTier\s{tierLevel}.;tmi-sent-ts=1654696969696;" +
            $"user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        // Subscribe (Prime)
        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/0;badges=subscriber/0;color=;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=sub;msg-param-cumulative-months=1;msg-param-goal-contribution-type=SUB_POINTS;" +
            @"msg-param-goal-current-contributions=1211;msg-param-goal-description=I\shave\sno\sidea;msg-param-goal-target-contributions=1200;" +
            "msg-param-goal-user-contributions=1;msg-param-months=0;msg-param-multimonth-duration=1;msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;" +
            @"msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan=Prime;msg-param-was-gifted=false;room-id=00000000;subscriber=1;" +
            $@"system-msg={DUMMY_USERNAME}\ssubscribed\swith\sPrime.;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        // Re-Subscribe
        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/16;badges=subscriber/12;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=resub;msg-param-cumulative-months=16;msg-param-months=0;msg-param-multimonth-duration=0;" +
            $"msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;msg-param-sub-plan-name=Channel\\sSubscription\\sPOG;msg-param-sub-plan={ircCode};msg-param-was-gifted=false;" +
            $@"room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\ssubscribed\sat\sTier\s{tierLevel}.\sThey've\ssubscribed\sfor\s16\smonths!;tmi-sent-ts=1654696969696;" +
            $"user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL} :Hello World!"
        );

        // Re-Subscribe (Prime)
        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/23;badges=subscriber/18;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;" +
            $"id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=resub;msg-param-cumulative-months=23;msg-param-months=0;msg-param-multimonth-duration=0;" +
            "msg-param-multimonth-tenure=0;msg-param-should-share-streak=0;msg-param-sub-plan-name=Channel\\sSubscription\\sPOG;msg-param-sub-plan=Prime;" +
            $@"msg-param-was-gifted=false;room-id=000000000;subscriber=1;system-msg={DUMMY_USERNAME}\ssubscribed\swith\sPrime.\sThey've\ssubscribed\sfor\s23\smonths!;" +
            $"tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL} :Hello World!"
        );

        // Gift subscription to a user from a known user
        await _client.SimulateMessagesAsync(
            $"@badge-info=subscriber/12;badges=subscriber/12;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};" +
            @$"login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=1;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;" +
            $"msg-param-recipient-display-name={DUMMY_USERNAME}_Foobar;msg-param-recipient-id=000000000;msg-param-recipient-user-name={DUMMY_USERNAME}_Foobar;msg-param-sender-count=50;" +
            $"msg-param-sub-plan-name=Channel\\sSubscription\\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;" +
            $@"system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_Foobar!\sThey\shave\sgiven\s50\sGift\sSubs\sin\sthe\schannel!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};" +
            $"user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        // Gift subscription to a user from an anonymous user
        await _client.SimulateMessagesAsync(
            $"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};" +
            $"login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringFive;msg-param-gift-months=1;msg-param-months=11;" +
            @$"msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME};" +
            @$"msg-param-recipient-id={DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME};msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;" +
            @$"subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        // Community gift subscriptions from a known user (5 subscriptions)
        await _client.SimulateMessagesAsync(
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=submysterygift;msg-param-goal-contribution-type=SUB_POINTS;msg-param-goal-current-contributions=497;msg-param-goal-description=nice;msg-param-goal-target-contributions=699;msg-param-goal-user-contributions=5;msg-param-mass-gift-count=5;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-sender-count=123;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sis\sgifting\s5\sTier\s{tierLevel}\sSubs\sto\sPOG's\scommunity!\sThey've\sgifted\sa\stotal\sof\s123\sin\sthe\schannel!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=2;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_A;msg-param-recipient-id=1{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_A;msg-param-sender-count=0;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_A!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=5;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_B;msg-param-recipient-id=2{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_B;msg-param-sender-count=0;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_B!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=2;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_C;msg-param-recipient-id=3{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_C;msg-param-sender-count=0;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_C!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=8;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_D;msg-param-recipient-id=4{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_D;msg-param-sender-count=0;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_D!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            $@"@badge-info=founder/31;badges=founder/0;color=#121212;display-name={DUMMY_USERNAME};emotes=;flags=;id={NewGuid};login={DUMMY_USERNAME};mod=0;msg-id=subgift;msg-param-gift-months=1;msg-param-months=1;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_E;msg-param-recipient-id=5{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_E;msg-param-sender-count=0;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=1;system-msg={DUMMY_USERNAME}\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_E!;tmi-sent-ts=1654696969696;user-id={DUMMY_USER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        // Community gift subscriptions from an anonymous user (5 subscriptions)
        await _client.SimulateMessagesAsync(
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=submysterygift;msg-param-mass-gift-count=5;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sis\sgifting\s5\sTier\s{tierLevel}\sSubs\sto\sPOG's\scommunity!;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringFive;msg-param-gift-months=1;msg-param-months=1;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_A;msg-param-recipient-id=1{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_A;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_A!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringThree;msg-param-gift-months=1;msg-param-months=5;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_B;msg-param-recipient-id=2{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_B;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_B!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringFour;msg-param-gift-months=1;msg-param-months=1;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_C;msg-param-recipient-id=3{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_C;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_C!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringTwo;msg-param-gift-months=1;msg-param-months=6;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_D;msg-param-recipient-id=4{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_D;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_D!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}",
            @$"@badge-info=;badges=;color=;display-name={KnownLogins.AN_ANONYMOUS_GIFTER_DISPLAY};emotes=;flags=;id={NewGuid};login={KnownLogins.AN_ANONYMOUS_GIFTER};mod=0;msg-id=subgift;msg-param-fun-string=FunStringFive;msg-param-gift-months=1;msg-param-months=9;msg-param-origin-id=00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00\s00;msg-param-recipient-display-name={DUMMY_USERNAME}_E;msg-param-recipient-id=5{DUMMY_USER_ID};msg-param-recipient-user-name={DUMMY_USERNAME}_E;msg-param-sub-plan-name=Channel\sSubscription\sPOG;msg-param-sub-plan={ircCode};room-id=00000000;subscriber=0;system-msg=An\sanonymous\suser\sgifted\sa\sTier\s{tierLevel}\ssub\sto\s{DUMMY_USERNAME}_E!\s;tmi-sent-ts=1654696969696;user-id={KnownLogins.AN_ANONYMOUS_GIFTER_ID};user-type= :tmi.twitch.tv {KnownCommands.USERNOTICE} #{DUMMY_CHANNEL}"
        );

        Assert.Multiple(() =>
        {
            Assert.That(counter, Is.EqualTo(16), "Counter");
            Assert.That(flag.Wait(), Is.True, "Event was not raised!");
        });
    }
}