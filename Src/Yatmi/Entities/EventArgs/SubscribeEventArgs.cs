using System;
using Yatmi.Enum;

namespace Yatmi.Entities.EventArgs
{
    public class SubscribeEventArgs : BaseEventArgs
    {
        /// <summary>
        /// In which channel this event occurred
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Username of who that decided to subscribe to the channel
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// How long they subscribed for
        /// </summary>
        public int Months { get; }

        /// <summary>
        /// What tier of subscription they paid for
        /// </summary>
        public SubPlanTypes SubPlanType { get; }

        /// <summary>
        /// The system message text from Twitch
        /// </summary>
        public string SystemMessage { get; }


        public SubscribeEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string channel,
            string username,
            int months,
            string subPlan,
            string systemMessage
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Channel = channel;
            Username = username;
            Months = months;
            SubPlanType = Helper.GetSubPlanType(subPlan);
            SystemMessage = systemMessage;
        }
    }
}
