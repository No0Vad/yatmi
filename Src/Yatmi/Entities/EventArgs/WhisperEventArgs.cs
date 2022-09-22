using System;

namespace Yatmi.Entities.EventArgs
{
    public class WhisperEventArgs : BaseEventArgs
    {
        /// <summary>
        /// User who send the bot a whisper message
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// The message that was sent
        /// </summary>
        public string Message { get; }


        public WhisperEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string username,
            string message
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Username = username;
            Message = message;
        }
    }
}
