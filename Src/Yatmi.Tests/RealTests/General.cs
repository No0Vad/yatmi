namespace Yatmi.Tests.RealTests;

public class General : TestBase
{
    [Test]
    public async Task Connectivity()
    {
        var flag = new Flag(2);

        _client.OnConnected += (s, e) =>
        {
            Assert.That(_client.IsConnected, Is.EqualTo(true), "IsConnected");

            flag.Set();
        };

        _client.OnDisconnected += (s, e) =>
        {
            Assert.That(_client.IsConnected, Is.EqualTo(false), "IsConnected");

            flag.Set();
        };

        await _client.ConnectAsync();

        await Task.Delay(1000);

        await _client.DisconnectAsync();

        Assert.That(flag.Wait(), Is.True, "Event was not raised!");
    }
}