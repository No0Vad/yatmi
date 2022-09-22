using Yatmi.Enum;

namespace Yatmi.Tests.SimulatedTests
{
    public class Moderation : TestBase
    {
        [Test]
        [TestCase(2)]
        [TestCase(60)]
        [TestCase(120)]
        public async Task OnUserTimeout(int seconds)
        {
            var flag = new Flag();

            _client.OnUserTimeout += (s, e) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                    Assert.That(e.TimedoutUsername, Is.EqualTo(DUMMY_USERNAME), "TimedoutUsername");
                    Assert.That(e.Duration, Is.EqualTo(TimeSpan.FromSeconds(seconds)), "Duration");
                });

                flag.Set();
            };

            await _client.SimulateMessagesAsync(
                $"@ban-duration={seconds};room-id=00000000;target-user-id=000000000;tmi-sent-ts=1656969696969 :tmi.twitch.tv {KnownCommands.CLEARCHAT} #{DUMMY_CHANNEL} :{DUMMY_USERNAME}"
            );

            Assert.That(flag.Wait(), Is.True, "Event was not raised!");
        }

        [Test]
        public async Task OnUserBanned()
        {
            var flag = new Flag();

            _client.OnUserBanned += (s, e) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                    Assert.That(e.BannedUsername, Is.EqualTo(DUMMY_USERNAME), "BannedUsername");
                });

                flag.Set();
            };

            await _client.SimulateMessagesAsync(
                $"@room-id=00000000;target-user-id=000000000;tmi-sent-ts=1656969696969 :tmi.twitch.tv {KnownCommands.CLEARCHAT} #{DUMMY_CHANNEL} :{DUMMY_USERNAME}"
            );

            Assert.That(flag.Wait(), Is.True, "Event was not raised!");
        }

        [Test]
        public async Task OnChatCleared()
        {
            var flag = new Flag();

            _client.OnChatCleared += (s, e) =>
            {
                Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");

                flag.Set();
            };

            await _client.SimulateMessagesAsync(
                $"@room-id=00000000;tmi-sent-ts=000000000 :tmi.twitch.tv {KnownCommands.CLEARCHAT} #{DUMMY_CHANNEL}"
            );

            Assert.That(flag.Wait(), Is.True, "Event was not raised!");
        }

        [Test]
        public async Task OnChatMessageDeleted()
        {
            var flag = new Flag();

            _client.OnChatMessageDeleted += (s, e) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(e.Channel, Is.EqualTo(DUMMY_CHANNEL), "Channel");
                    Assert.That(e.DeletedMessage, Is.EqualTo("Bad message!"), "DeletedMessage");
                });

                flag.Set();
            };

            await _client.SimulateMessagesAsync(
                $"@login={DUMMY_USERNAME};room-id=;target-msg-id={GUID};" +
                $"tmi-sent-ts=1656969696969 :tmi.twitch.tv {KnownCommands.CLEARMSG} #{DUMMY_CHANNEL} :Bad message!"
            );

            Assert.That(flag.Wait(), Is.True, "Event was not raised!");
        }
    }
}
