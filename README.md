# YATMI

## About
Yatmi (Yet Another Twitch Messaging Interface) allows you to connect to a Twitch Chat and read message or respond to messages.

## Why?
I was bored and wanted a challenge.

## Quick start
```csharp
var client = new TwitchChatClient();

// Or if you want to be able to send chat messages
// var client = new TwitchChatClient("botName", "botOauthToken");

client.OnChatMessage += (sender, e) => 
{
    Console.WriteLine(string.Join(" ", new[]
    {
        $"[{e.Timestamp}]",
        $"[{e.Channel}]",
        $"{e.Username} wrote:",
        e.Message
    }));
};

await client.ConnectAsync();

await client.JoinChannelAsync("channel_name");

// To disconnect and free up resources
// await client.DisposeAsync()
```

## Scopes
If you are using a username + oath token, you require some scopes for it to work.

|      Scope       |                                   Description                                    |
| ---------------- | -------------------------------------------------------------------------------- |
| channel:moderate | Use moderator commands. Making your bot a moderator is not enough, you need both |
| chat:read        | View chat messages                                                               |
| chat:edit        | Send chat messages                                                               |
| whispers:read    | View your whisper messages                                                       |
| whispers:edit    | Send whisper messages                                                            |

You can read more about the chat scopes here: https://dev.twitch.tv/docs/authentication/scopes

### Regarding whispers
To send a whisper your bot needs to be a *known bot* by Twitch. But it can still receive a whisper as long as it has the **whispers:read** scope for it.

Because of that I have not been able to test and verify that sending a whisper actually works.