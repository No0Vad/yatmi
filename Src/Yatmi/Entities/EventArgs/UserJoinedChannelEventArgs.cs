using System;

namespace Yatmi.Entities.EventArgs
{
    public class UserJoinedChannelEventArgs : BaseEventArgs
    {
        /// <summary>
        /// Name of the channel the user just joined
        /// </summary>
        public string JoinedChannel { get; }

        /// <summary>
        /// Username of the user who just joined
        /// </summary>
        public string Username { get; }


        public UserJoinedChannelEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string joinedChannel,
            string username
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            JoinedChannel = joinedChannel;
            Username = username;
        }
    }
}
