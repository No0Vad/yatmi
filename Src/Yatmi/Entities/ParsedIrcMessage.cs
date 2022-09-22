using System;
using Yatmi.Enum;

namespace Yatmi.Entities
{
    public class ParsedIrcMessage
    {
        private DateTime? _timestamp;
        private readonly DateTime _created;

        /// <summary>
        /// If <see cref="TwitchChatClient.KeepRawIrcMessageAfterParsed"/> is true, the raw IRC message is stored here. If not, this is null
        /// </summary>
        public string RawIrcMessage { get; internal set; }

        /// <summary>
        /// The code of the IRC message
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// The chat message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The username of the message
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// In which channel this message was sent
        /// </summary>
        public string Channel { get; private set; }

        /// <summary>
        /// A collection of tags from the IRC message
        /// </summary>
        public Tags Tags { get; private set; }

        /// <summary>
        /// Timestamp of when the message was sent
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return _timestamp ??= Tags?.TryGetValue(KnownTags.SENT_TIMESTAMP, out var value) == true && long.TryParse(value, out var longValue)
                    ? DateTimeOffset.FromUnixTimeMilliseconds(longValue).UtcDateTime
                    : _created;
            }
        }


        /// <summary>
        /// Creates a new instance of <see cref="ParsedIrcMessage" />
        /// </summary>
        public ParsedIrcMessage()
        {
            _created = DateTime.UtcNow;
        }


        /// <summary>
        /// Parse the raw IRC message and creates a new <see cref="ParsedIrcMessage"/>
        /// </summary>
        /// <param name="rawMessage">The raw IRC message</param>
        /// <param name="keepRawIrcMessageAfterParsed">If true, the raw IRC message will be kept. Otherwise <see cref="RawIrcMessage"/> will remain null. Default is false</param>
        public static ParsedIrcMessage Parse(string rawMessage, bool keepRawIrcMessageAfterParsed = false)
        {
            try
            {
                var entity = InternalParse(rawMessage);

                if (entity != null && keepRawIrcMessageAfterParsed)
                {
                    entity.RawIrcMessage = rawMessage;
                }

                return entity;
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Internal parser of the raw IRC message
        /// </summary>
        /// <param name="rawMessage">The raw IRC message</param>
        private static ParsedIrcMessage InternalParse(string rawMessage)
        {
            if (string.IsNullOrEmpty(rawMessage))
            {
                return null;
            }

            var span = rawMessage.AsSpan();

            if (span.Length > 4 && span[0] == 'P' && span[1] == 'I' && span[2] == 'N' && span[3] == 'G')
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.PING,
                    Message = span[5..].ToString()
                };
            }

            var isExtended = span[0] == '@';
            string tagsMessage;
            string normalMessage;
            var chatMessage = "";

            if (isExtended)
            {
                var tagSplitIndex = span.IndexOf(' ');
                tagsMessage = span[..tagSplitIndex].ToString();
                normalMessage = span[(tagSplitIndex + 2)..].ToString();
            }
            else
            {
                tagsMessage = "";
                normalMessage = rawMessage[1..];
            }

            var spanNormal = normalMessage.AsSpan();

            var index = spanNormal.IndexOf(':');
            if (index != -1)
            {
                chatMessage = spanNormal[(index + 1)..].ToString();
                spanNormal = spanNormal[..(index - 1)];
            }

            var parts = spanNormal.ToString().Split(' ');

            // Local helper for getting the username
            var getUsername = (string input) =>
            {
                var span = input.AsSpan();
                var needle = span.IndexOf('!');
                if (needle != -1)
                {
                    return span[..needle].ToString();
                }

                return "";
            };

            // Local helper for getting a clean channel name
            var getCleanChannel = (string input) => input[1..];

            if (parts[1] == KnownCommands.PRIVMSG)
            {
                var spanChatMessage = chatMessage.AsSpan();
                var tags = Tags.Parse(tagsMessage);

                // The /me action starts with \u0001ACTION and ends with \u0001
                if (spanChatMessage.Length > 0 && (byte)spanChatMessage[0] == 1 && (byte)spanChatMessage[chatMessage.Length - 1] == 1)
                {
                    chatMessage = spanChatMessage[8..(spanChatMessage.Length - 1)].ToString();

                    tags.Add(KnownTags.YATMI_IS_ME, "1");
                }

                return new ParsedIrcMessage
                {
                    Code = KnownCommands.PRIVMSG,
                    Username = getUsername(parts[0]),
                    Channel = getCleanChannel(parts[2]),
                    Message = chatMessage,
                    Tags = tags
                };
            }
            else if (parts[1] == KnownCommands.NOTICE)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.NOTICE,
                    Channel = getCleanChannel(parts[2]),
                    Message = chatMessage,
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.USERNOTICE)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.USERNOTICE,
                    Channel = getCleanChannel(parts[2]),
                    Message = chatMessage,
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.WHISPER)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.WHISPER,
                    Username = getUsername(parts[0]),
                    Channel = parts[2],
                    Message = chatMessage,
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.ROOMSTATE)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.ROOMSTATE,
                    Channel = getCleanChannel(parts[2]),
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.CLEARCHAT)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.CLEARCHAT,
                    Channel = getCleanChannel(parts[2]),
                    Message = chatMessage,
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.CLEARMSG)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.CLEARMSG,
                    Channel = getCleanChannel(parts[2]),
                    Message = chatMessage,
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == "001" || parts[1] == "002" || parts[1] == "003" || parts[1] == "004" || parts[1] == "375" || parts[1] == "372" || parts[1] == "376")
            {
                return new ParsedIrcMessage
                {
                    Code = parts[1],
                    Message = chatMessage,
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.CAP)
            {
                var tags = Tags.Parse(tagsMessage);
                tags.Add(KnownTags.YATMI_CAP_RESULT, parts[3]);

                return new ParsedIrcMessage
                {
                    Code = KnownCommands.CAP,
                    Message = chatMessage,
                    Tags = tags
                };
            }
            else if (parts[1] == KnownCommands.JOIN)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.JOIN,
                    Username = getUsername(parts[0]),
                    Channel = getCleanChannel(parts[2]),
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.PART)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.PART,
                    Username = getUsername(parts[0]),
                    Channel = getCleanChannel(parts[2]),
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.GLOBALUSERSTATE)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.GLOBALUSERSTATE,
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.USERSTATE)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.USERSTATE,
                    Channel = getCleanChannel(parts[2]),
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.NAMES_LIST)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.NAMES_LIST,
                    Username = parts[2],
                    Channel = getCleanChannel(parts[4]),
                    Message = chatMessage,
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.END_OF_NAMES_LIST)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.END_OF_NAMES_LIST,
                    Username = parts[2],
                    Channel = getCleanChannel(parts[3]),
                    Message = chatMessage,
                    Tags = Tags.Parse(tagsMessage)
                };
            }
            else if (parts[1] == KnownCommands.HOSTTARGET)
            {
                // A normal split should be enough.

                var tags = Tags.Parse(tagsMessage);
                var hostTargetParts = chatMessage.Split(' ');

                tags.Add(KnownTags.YATMI_HOST_TARGET, hostTargetParts[0]);
                tags.Add(KnownTags.YATMI_HOST_VIEWERS, hostTargetParts[1]);

                return new ParsedIrcMessage
                {
                    Code = KnownCommands.HOSTTARGET,
                    Channel = getCleanChannel(parts[2]),
                    Tags = tags
                };
            }
            else if (parts[1] == KnownCommands.RECONNECT)
            {
                return new ParsedIrcMessage
                {
                    Code = KnownCommands.RECONNECT,
                    Tags = Tags.Parse(tagsMessage)
                };
            }

            return null;
        }
    }
}