using System;

namespace Yatmi.Entities.EventArgs
{
    public class UserTimeoutEventArgs : BaseEventArgs
    {
        /// <summary>
        /// In which channel this event occurred
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Username of who that was timed out
        /// </summary>
        public string TimedoutUsername { get; }

        /// <summary>
        /// How long they were timed out for
        /// </summary>
        public TimeSpan Duration { get; }


        public UserTimeoutEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string channel,
            string timedoutUsername,
            int durationInSeconds
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Channel = channel;
            TimedoutUsername = timedoutUsername;
            Duration = TimeSpan.FromSeconds(durationInSeconds);
        }
    }
}
