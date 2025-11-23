namespace Yatmi.Enum
{
    public static class KnownTags
    {
        public const string ID = "id";
        public const string USER_ID = "user-id";
        public const string USER_TYPE = "user-type";
        public const string BADGES = "badges";
        public const string COLOR = "color";
        public const string SENT_TIMESTAMP = "tmi-sent-ts";
        public const string MSG_ID = "msg-id";
        public const string LOGIN = "login";
        public const string MOD = "mod";
        public const string DISPLAY_NAME = "display-name";
        public const string FIRST_MSG = "first-msg";
        public const string BITS = "bits";
        public const string EMOTES = "emotes";
        public const string FLAGS = "flags";
        public const string TURBO = "turbo";
        public const string ROOM_ID = "room-id";
        public const string SOURCE_ROOM_ID = "source-room-id";
        public const string TMI_SENT_TS = "tmi-sent-ts";

        public const string MSG_PARAM_VIEWER_COUNT = "msg-param-viewerCount";
        public const string MSG_PARAM_CUMULATIVE_MONTHS = "msg-param-cumulative-months";
        public const string MSG_PARAM_SUB_PLAN = "msg-param-sub-plan";
        public const string MSG_PARAM_SENDER_COUNT = "msg-param-sender-count";
        public const string MSG_PARAM_SENDER_LOGIN = "msg-param-sender-login";
        public const string MSG_PARAM_RECIPIENT_USERNAME = "msg-param-recipient-user-name";
        public const string MSG_PARAM_RECIPIENT_USER_ID = "msg-param-recipient-id";
        public const string MSG_PARAM_MASS_GIFT_COUNT = "msg-param-mass-gift-count";
        public const string MSG_PARAM_MASS_ORIGIN_ID = "msg-param-origin-id";
        public const string MSG_PARAM_GIFTER_USERNAME = "msg-param-prior-gifter-user-name";
        public const string MSG_PARAM_GIFTER_USER_ID = "msg-param-prior-gifter-id";
        public const string MSG_PARAM_THRESHOLD = "msg-param-threshold";
        public const string MSG_PARAM_AMOUNT = "msg-param-amount";
        public const string MSG_PARAM_CURRENCY = "msg-param-currency";
        public const string MSG_PARAM_CATEGORY = "msg-param-category";
        public const string MSG_PARAM_VALUE = "msg-param-value";

        // All related to OneTap
        public const string MSG_PARAM_BREAKPOINT_NUMBER = "msg-param-breakpoint-number";
        public const string MSG_PARAM_BREAKPOINT_THRESHOLD_BITS = "msg-param-breakpoint-threshold-bits";
        public const string MSG_PARAM_GIFT_ID = "msg-param-gift-id";
        public const string MSG_PARAM_CHANNEL_DISPLAY_NAME = "msg-param-channel-display-name";
        public const string MSG_PARAM_CONTRIBUTOR_1 = "msg-param-contributor-1";
        public const string MSG_PARAM_CONTRIBUTOR_1_TAPS = "msg-param-contributor-1-taps";
        public const string MSG_PARAM_CONTRIBUTOR_2 = "msg-param-contributor-2";
        public const string MSG_PARAM_CONTRIBUTOR_2_TAPS = "msg-param-contributor-2-taps";
        public const string MSG_PARAM_LARGEST_CONTRIBUTOR_COUNT = "msg-param-largest-contributor-count";
        public const string MSG_PARAM_STREAK_SIZE_BITS = "msg-param-streak-size-bits";
        public const string MSG_PARAM_STREAK_SIZE_TAPS = "msg-param-streak-size-taps";
        public const string MSG_PARAM_MS_REMAINING = "msg-param-ms-remaining";
        public const string MSG_PARAM_BITS_SPENT = "msg-param-bits-spent";
        public const string MSG_PARAM_USER_DISPLAY_NAME = "msg-param-user-display-name";

        public const string REPLY_PARENT_MSG_ID = "reply-parent-msg-id";
        public const string REPLY_PARENT_MSG_USER_ID = "reply-parent-user-id";
        public const string REPLY_PARENT_MSG_USER_LOGIN = "reply-parent-user-login";
        public const string REPLY_PARENT_MSG_BODY = "reply-parent-msg-body";

        public const string REPLY_THREAD_PARENT_MSG_ID = "reply-thread-parent-msg-id";
        public const string REPLY_THREAD_PARENT_USER_ID = "reply-thread-parent-user-id";
        public const string REPLY_THREAD_PARENT_USER_LOGIN = "reply-thread-parent-user-login";

        public const string CUSTOM_REWARD_ID = "custom-reward-id";
        public const string SYSTEM_MSG = "system-msg";
        public const string TARGET_USER_ID = "target-user-id";
        public const string TARGET_MESSAGE_ID = "target-msg-id";
        public const string BAN_DURATION = "ban-duration";

        public const string EMOTES_ONLY = "emote-only";
        public const string FOLLOWERS_ONLY = "followers-only";
        public const string SLOW = "slow";
        public const string SUBS_ONLY = "subs-only";

        public const string RETURNING_CHATTER = "returning-chatter";

        public const string PINNED_CHAT_PAID_AMOUNT = "pinned-chat-paid-amount";
        public const string PINNED_CHAT_PAID_CANONICAL_AMOUNT = "pinned-chat-paid-canonical-amount";
        public const string PINNED_CHAT_PAID_CURRENCY = "pinned-chat-paid-currency";
        public const string PINNED_CHAT_PAID_EXPONENT = "pinned-chat-paid-exponent";
        public const string PINNED_CHAT_PAID_IS_SYSTEM_MESSAGE = "pinned-chat-paid-is-system-message";
        public const string PINNED_CHAT_PAID_LEVEL = "pinned-chat-paid-level";


        // Test are custom ones because use the tags for custom data
        public const string YATMI_IS_ME = "yatmi-is-me";
        public const string YATMI_HOST_TARGET = "yatmi-host-target";
        public const string YATMI_HOST_VIEWERS = "yatmi-host-viewers";
        public const string YATMI_CAP_RESULT = "yatmi-cap-result";
    }
}