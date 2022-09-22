using System;

namespace Yatmi.Entities.EventArgs
{
    public class FollowersOnlyStateEventArgs : BaseEventArgs
    {
        /// <summary>
        /// In which channel this event occurred
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// True if followers only is active
        /// </summary>
        public bool FollowersOnly { get; }

        /// <summary>
        /// How long the user must have been a follower.
        /// </summary>
        public TimeSpan FollowersTime { get; }


        public FollowersOnlyStateEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string channel,
            int followersOnly
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Channel = channel;

            if (followersOnly == -1)
            {
                FollowersOnly = false;
                FollowersTime = default;
            }
            else
            {
                FollowersOnly = true;
                FollowersTime = TimeSpan.FromMinutes(followersOnly);
            }
        }
    }
}