using System;

namespace Yatmi.Entities.EventArgs
{
    public class TimestampEventArgs : System.EventArgs
    {
        /// <summary>
        /// Timestamp of when this event was created or it the <see cref="ParsedIrcMessage"/> contains a timestamp, that is used instead
        /// </summary>
        public DateTime Timestamp { get; }


        /// <summary>
        /// Creates a new instance of <see cref="TimestampEventArgs"/> using current DateTime.UtcNow
        /// </summary>
        public TimestampEventArgs()
        {
            Timestamp = DateTime.UtcNow;
        }


        /// <summary>
        /// Creates a new instance of <see cref="TimestampEventArgs"/> using timestamp from the <see cref="ParsedIrcMessage"/>
        /// </summary>
        /// <param name="timestamp">The timestamp to use</param>
        public TimestampEventArgs(DateTime timestamp)
        {
            Timestamp = timestamp;
        }
    }
}