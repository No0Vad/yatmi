using System;

namespace Yatmi.Entities.EventArgs
{
    public class NamesListEventArgs : BaseEventArgs
    {
        /// <summary>
        /// In which channel this event occurred
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// The current users in the channel
        /// </summary>
        public string[] Usernames { get; }


        public NamesListEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string channel,
            string message
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Channel = channel;
            Usernames = message.Split(' ');
        }
    }
}
