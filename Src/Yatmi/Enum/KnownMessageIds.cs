namespace Yatmi.Enum;

/// <summary>
/// More details here https://dev.twitch.tv/docs/irc/msg-id
/// --
/// I've added the ones needed for correct events and the most common ones in my experience when testing.
/// </summary>
public static class KnownMessageIds
{
    public const string RAID = "raid";
    public const string UNRAID = "unraid";
    public const string ANNOUNCEMENT = "announcement";
    public const string HIGHLIGHTED_MESSAGE = "highlighted-message";
    public const string USER_INTRO = "user-intro";
    public const string SUB = "sub";
    public const string RESUB = "resub";
    public const string SUBGIFT = "subgift";
    public const string MYSTERY_SUBGIFT = "submysterygift";
    public const string GIFT_PAID_UPGRADE = "giftpaidupgrade";
    public const string COMMUNITY_PAY_FORWARD = "communitypayforward";
    public const string STANDARD_PAY_FORWARD = "standardpayforward";
    public const string PRIME_PAID_UPGRADE = "primepaidupgrade";
    public const string BITS_BADGE_TIER = "bitsbadgetier";
    public const string ANON_GIFT_PAID_UPGRADE = "anongiftpaidupgrade";
    public const string NO_PERMISSION = "no_permission";
    public const string MSG_RATELIMIT = "msg_ratelimit";
    public const string MSG_SUBSONLY = "msg_subsonly";
    public const string MSG_FOLLWERS_ONLY = "msg_followersonly";
    public const string MSG_EMOTES_ONLY = "msg_emoteonly";
    public const string MSG_CHANNEL_SUSPENDED = "msg_channel_suspended";
    public const string GIGANTIFIED_EMOTE_MESSAGE = "gigantified-emote-message";
    public const string ANIMATED_MESSAGE = "animated-message";

    /// <summary>
    /// Elevated message, paid to be shown longer
    /// </summary>
    public const string MIDNIGHTSQUID = "midnightsquid";
    public const string VIEWERMILESTONE = "viewermilestone";

    public const string TIMEOUT_SUCCESS = "timeout_success";
    public const string BAN_SUCCESS = "ban_success";
    public const string DELETE_MESSAGE_SUCCESS = "delete_message_success";

    public const string FOLLOWERS_ON_ZERO = "followers_on_zero";
    public const string FOLLOWERS_ON = "followers_on";
    public const string FOLLOWERS_OFF = "followers_off";
    public const string SUBS_ON = "subs_on";
    public const string SUBS_OFF = "subs_off";
    public const string EMOTES_ONLY_ON = "emote_only_on";
    public const string EMOTES_ONLY_OFF = "emote_only_off";
    public const string SLOW_ON = "slow_on";
    public const string SLOW_OFF = "slow_off";
    public const string HOST_ON = "host_on";
    public const string HOST_OFF = "host_off";
    public const string HOST_TARGET_WENT_OFFLINE = "host_target_went_offline";
}
