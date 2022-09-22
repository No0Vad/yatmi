using System;

namespace Yatmi.Entities.EventArgs
{
    public class HostEventArgs : BaseEventArgs
    {
        /// <summary>
        /// In which channel this event occurred
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// The channel that was hosted
        /// </summary>
        public string Hosted { get; }

        /// <summary>
        /// How many viewers there was
        /// </summary>
        public int Viewers { get; }

        /// <summary>
        /// If true, a host is active
        /// </summary>
        public bool IsHosting => Hosted != "-";


        public HostEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string channel,
            string hosted,
            int viewers
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Channel = channel;
            Hosted = hosted;
            Viewers = viewers;
        }

    }
}
