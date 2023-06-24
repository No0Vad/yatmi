namespace Yatmi.Enum;

/// <summary>
/// More details here https://dev.twitch.tv/docs/irc/commands
/// --
/// I've added the ones needed for correct events and the most common ones in my experience when testing.
/// </summary>
public static class KnownCommands
{
    public const string PING = "PING";
    public const string CAP = "CAP";
    public const string JOIN = "JOIN";
    public const string PART = "PART";
    public const string ROOMSTATE = "ROOMSTATE";
    public const string PRIVMSG = "PRIVMSG";
    public const string USERNOTICE = "USERNOTICE";
    public const string WHISPER = "WHISPER";
    public const string NOTICE = "NOTICE";
    public const string GLOBALUSERSTATE = "GLOBALUSERSTATE";
    public const string USERSTATE = "USERSTATE";
    public const string CLEARCHAT = "CLEARCHAT";
    public const string CLEARMSG = "CLEARMSG";
    public const string RECONNECT = "RECONNECT";
    public const string HOSTTARGET = "HOSTTARGET";

    public const string NAMES_LIST = "353";
    public const string END_OF_NAMES_LIST = "366";
}
