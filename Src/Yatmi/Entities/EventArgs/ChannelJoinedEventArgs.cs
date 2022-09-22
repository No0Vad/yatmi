using System;

namespace Yatmi.Entities.EventArgs
{
    public class ChannelJoinedEventArgs : BaseEventArgs
    {
        /// <summary>
        /// Name of the channel the bot just joined
        /// </summary>
        public string JoinedChannel { get; }


        public ChannelJoinedEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string joinedChannel
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            JoinedChannel = joinedChannel;
        }
    }
}