using System;

namespace Yatmi.Entities.EventArgs
{
    public class SlowModeStateEventArgs : BaseEventArgs
    {
        /// <summary>
        /// In which channel this event occurred
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// How long you must wait before sending a new message
        /// </summary>
        public TimeSpan SlowMode { get; }


        public SlowModeStateEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string channel,
            int slowMode
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Channel = channel;
            SlowMode = TimeSpan.FromSeconds(slowMode);
        }
    }
}
