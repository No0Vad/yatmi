using System;

namespace Yatmi.Entities.EventArgs
{
    public class ChatClearedEventArgs : BaseEventArgs
    {
        /// <summary>
        /// In which channel this event occurred
        /// </summary>
        public string Channel { get; }


        public ChatClearedEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string channel
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Channel = channel;
        }
    }
}