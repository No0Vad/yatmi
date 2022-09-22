using System;

namespace Yatmi.Entities.EventArgs
{
    public class SubscriptionPayForwardEventArgs : BaseEventArgs
    {
        /// <summary>
        /// In which channel this event occurred
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Username of who that decided to pay forward their subscription they were gifted, but gifting a subscription to someone else
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Username who they originally got a gift subscription from
        /// </summary>
        public string GifterUsername { get; }

        /// <summary>
        /// Username who they decided to gift a subscription to
        /// </summary>
        public string ReceiverUsername { get; }

        /// <summary>
        /// The system message text from Twitch
        /// </summary>
        public string SystemMessage { get; }


        public SubscriptionPayForwardEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string channel,
            string username,
            string gifterUsername,
            string receiverUsername,
            string systemMessage
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Channel = channel;
            Username = username;
            GifterUsername = gifterUsername;
            ReceiverUsername = receiverUsername;
            SystemMessage = systemMessage;
        }
    }
}
