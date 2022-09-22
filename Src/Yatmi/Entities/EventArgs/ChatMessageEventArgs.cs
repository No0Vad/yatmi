using System;
using Yatmi.Enum;

namespace Yatmi.Entities.EventArgs
{
    public class ChatMessageEventArgs : BaseEventArgs
    {
        /// <summary>
        /// In which channel this event occurred
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Username of who sent the message
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// The actual message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// If <see cref="TwitchChatClient.ParseEmotesInMessages"/> is true, the emotes data is stored here. If not, this is null
        /// </summary>
        public Emotes Emotes { get; }

        /// <summary>
        /// How many bits that were used
        /// </summary>
        public int Bits { get; }

        /// <summary>
        /// The ID of the message
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// What type of message this is
        /// </summary>
        public MessageTypes MessageType { get; }

        /// <summary>
        /// Indicates if the sender is the streamer
        /// </summary>
        public bool IsBroadcaster { get; }

        /// <summary>
        /// Indicates if the sender is a moderator in the channel
        /// </summary>
        public bool IsModerator { get; }

        /// <summary>
        /// Indicates if the sender is a founder to the channel
        /// </summary>
        public bool IsFounder { get; }

        /// <summary>
        /// Indicates if the sender is a subscriber to the channel
        /// </summary>
        public bool IsSubscriber { get; }

        /// <summary>
        /// Indicates if the sender is VIP to the channel
        /// </summary>
        public bool IsVip { get; }

        /// <summary>
        /// Indicates if the message is using the /me command
        /// </summary>
        public bool IsMe { get; }

        /// <summary>
        /// Indicates if this was the senders first message in this channel
        /// </summary>
        public bool IsFirstMessage { get; }


        public ChatMessageEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string channel,
            string username,
            string message,
            string emotes,
            int bits,
            string id,
            string badges,
            string msgId,
            string customRewardId,
            bool isMe,
            bool isFirstMessage
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Channel = channel;
            Username = username;
            Message = message;
            Emotes = Emotes.Parse(emotes);
            Bits = bits;
            ID = id;

            if (string.IsNullOrEmpty(msgId) && !string.IsNullOrEmpty(customRewardId))
            {
                MessageType = MessageTypes.CustomReward;
            }
            else
            {
                MessageType = msgId switch
                {
                    KnownMessageIds.ANNOUNCEMENT => MessageTypes.Announcement,
                    KnownMessageIds.HIGHLIGHTED_MESSAGE => MessageTypes.Highlight,
                    KnownMessageIds.USER_INTRO => MessageTypes.UserIntroduction,
                    _ => MessageTypes.Normal
                };
            }

            if (badges != null)
            {
                var badgeSpan = badges.AsSpan();
                IsBroadcaster = badgeSpan.IndexOf(KnownBadges.BROADCASTER.AsSpan()) != -1;
                IsModerator = badgeSpan.IndexOf(KnownBadges.MODERATOR.AsSpan()) != -1;
                IsFounder = badgeSpan.IndexOf(KnownBadges.FOUNDER.AsSpan()) != -1;
                IsSubscriber = badgeSpan.IndexOf(KnownBadges.SUBSCRIBER.AsSpan()) != -1;
                IsVip = badgeSpan.IndexOf(KnownBadges.VIP.AsSpan()) != -1;
            }

            IsMe = isMe;
            IsFirstMessage = isFirstMessage;
        }
    }
}
