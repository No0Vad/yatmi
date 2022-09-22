using System;
using Yatmi.Enum;

namespace Yatmi.Entities.EventArgs
{
    public class PrimeUpgradeSubscriptionEventArgs : BaseEventArgs
    {
        /// <summary>
        /// In which channel this event occurred
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Username of who that decided to upgrade their subscription from prime to a occurring subscription
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// What tier of subscription they upgrade too
        /// </summary>
        public SubPlanTypes SubPlanType { get; }

        /// <summary>
        /// The system message text from Twitch
        /// </summary>
        public string SystemMessage { get; }


        public PrimeUpgradeSubscriptionEventArgs(
            ParsedIrcMessage parsedIrcMessage,
            DateTime timestamp,
            string channel,
            string username,
            string subPlan,
            string systemMessage
        ) : base(
            parsedIrcMessage,
            timestamp
        )
        {
            Channel = channel;
            Username = username;
            SubPlanType = Helper.GetSubPlanType(subPlan);
            SystemMessage = systemMessage;
        }
    }
}
