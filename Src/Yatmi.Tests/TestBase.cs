namespace Yatmi.Tests;

public class TestBase
{
    protected TwitchChatClient _client;
    protected const string DUMMY_CHANNEL = "Best_Channel";
    protected const string DUMMY_USERNAME = "Best_User";

    protected static string GUID => Guid.NewGuid().ToString();

    [SetUp]
    public virtual void Setup()
    {
        _client = new TwitchChatClient();
    }


    [TearDown]
    public virtual async Task Teardown()
    {
        if (_client?.IsConnected == true)
        {
            await _client.DisconnectAsync();
        }
    }
}
