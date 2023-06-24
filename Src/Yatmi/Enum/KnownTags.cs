namespace Yatmi.Enum
{
    public static class KnownTags
    {
        public const string ID = "id";
        public const string BADGES = "badges";
        public const string COLOR = "color";
        public const string SENT_TIMESTAMP = "tmi-sent-ts";
        public const string MSG_ID = "msg-id";
        public const string LOGIN = "login";
        public const string DISPLAY_NAME = "display-name";
        public const string FIRST_MSG = "first-msg";
        public const string BITS = "bits";
        public const string EMOTES = "emotes";

        public const string MSG_PARAM_VIEWER_COUNT = "msg-param-viewerCount";
        public const string MSG_PARAM_CUMULATIVE_MONTHS = "msg-param-cumulative-months";
        public const string MSG_PARAM_SUB_PLAN = "msg-param-sub-plan";
        public const string MSG_PARAM_SENDER_COUNT = "msg-param-sender-count";
        public const string MSG_PARAM_SENDER_LOGIN = "msg-param-sender-login";
        public const string MSG_PARAM_RECIPIENT_USERNAME = "msg-param-recipient-user-name";
        public const string MSG_PARAM_MASS_GIFT_COUNT = "msg-param-mass-gift-count";
        public const string MSG_PARAM_MASS_ORIGIN_ID = "msg-param-origin-id";
        public const string MSG_PARAM_GIFTER_USERNAME = "msg-param-prior-gifter-user-name";
        public const string MSG_PARAM_THRESHOLD = "msg-param-threshold";
        public const string MSG_PARAM_AMOUNT = "msg-param-amount";
        public const string MSG_PARAM_CURRENCY = "msg-param-currency";
        public const string MSG_PARAM_CATEGORY = "msg-param-category";
        public const string MSG_PARAM_VALUE = "msg-param-value";

        public const string REPLY_PARENT_MSG_ID = "reply-parent-msg-id";

        public const string CUSTOM_REWARD_ID = "custom-reward-id";
        public const string SYSTEM_MSG = "system-msg";
        public const string TARGET_USER_ID = "target-user-id";
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