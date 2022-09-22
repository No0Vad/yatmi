using System.Text;
using Yatmi.Enum;

namespace Yatmi
{
    internal static class Helper
    {
        /// <summary>
        /// Helper for transforming the Twitch SubPlan values to <see cref="SubPlanTypes"/>
        /// </summary>
        public static SubPlanTypes GetSubPlanType(string subPlan)
        {
            return subPlan switch
            {
                "1000" => SubPlanTypes.Tier1,
                "2000" => SubPlanTypes.Tier2,
                "3000" => SubPlanTypes.Tier3,
                "Prime" => SubPlanTypes.Prime,
                _ => SubPlanTypes.Unknown
            };
        }


        /// <summary>
        /// Helper for returning a string from a bytes array
        /// </summary>
        /// <param name="bytes">The bytes to convert to a string</param>
        public static string GetString(byte[] bytes) => Encoding.UTF8.GetString(bytes);


        /// <summary>
        /// Helper for returning a bytes array from strings
        /// </summary>
        /// <param name="str">The string to convert to a bytes array</param>
        public static byte[] GetBytes(string str) => Encoding.UTF8.GetBytes(str);
    }
}