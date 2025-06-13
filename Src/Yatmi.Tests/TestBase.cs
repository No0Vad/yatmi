using Yatmi.Enum;

namespace Yatmi.Tests;

public class TestBase
{
    protected TwitchChatClient _client;
    protected const string DUMMY_CHANNEL = "Best_Channel";
    protected const string DUMMY_USERNAME = "Best_User";
    protected const string DUMMY_USER_ID = "00001337";

    /// <summary>
    /// On call, returns a new GUID
    /// </summary>
    protected static string NewGuid => Guid.NewGuid().ToString();

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
            await _client.DisposeAsync();
        }
    }


    protected static string Fabricate(string command, string msgID, Dictionary<string, object> kv)
    {
        return Fabricate(command, msgID, null, kv);
    }


    protected static string Fabricate(string command, string msgID, string text, Dictionary<string, object> kv)
    {
        var baseKv = new Dictionary<string, object>
        {
            { KnownTags.BADGES, "" },
            { KnownTags.COLOR, "#121212" },
            { KnownTags.EMOTES, "" },
            { KnownTags.FLAGS, "" },
            { KnownTags.ID, NewGuid },
            { KnownTags.MSG_ID, msgID },
            { KnownTags.DISPLAY_NAME, DUMMY_USERNAME },
            { KnownTags.LOGIN, DUMMY_USERNAME },
            { KnownTags.USER_ID, DUMMY_USER_ID },
            { KnownTags.USER_TYPE, "" },
            { KnownTags.MOD, 0 },
            { KnownTags.TMI_SENT_TS, 1656969696969 },
        };

        foreach (var item in kv)
        {
            baseKv[item.Key] = item.Value;
        }

        var ircMsg = string.Join(';', baseKv.Select(a => a.Key + "=" + a.Value)).Trim(';');

        if (string.IsNullOrEmpty(text))
        {
            return $"@{ircMsg} :tmi.twitch.tv {command} #{DUMMY_CHANNEL}";
        }

        return $"{ircMsg} :{DUMMY_USERNAME}!{DUMMY_USERNAME}@{DUMMY_USERNAME}.tmi.twitch.tv {command} #{DUMMY_CHANNEL} :@{text}";
    }
}
