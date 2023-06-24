using Yatmi.Enum;

namespace Yatmi.Entities;

public class PinnedChatPaidEntity
{
    public decimal Amount { get; }
    public decimal CanonicalAmount { get; }
    public string Currency { get; }
    public int Exponent { get; }
    public bool IsPaidSystemMessage { get; }
    public string Level { get; }

    public PinnedChatPaidEntity(Tags tags)
    {
        Amount = tags.GetIntValue(KnownTags.PINNED_CHAT_PAID_AMOUNT) / 100m;
        CanonicalAmount = tags.GetIntValue(KnownTags.PINNED_CHAT_PAID_CANONICAL_AMOUNT) / 100m;
        Currency = tags.GetStringValue(KnownTags.PINNED_CHAT_PAID_CURRENCY);
        Exponent = tags.GetIntValue(KnownTags.PINNED_CHAT_PAID_EXPONENT);
        IsPaidSystemMessage = tags.GetStringValue(KnownTags.PINNED_CHAT_PAID_IS_SYSTEM_MESSAGE) == "1";
        Level = tags.GetStringValue(KnownTags.PINNED_CHAT_PAID_LEVEL);
    }

    public PinnedChatPaidEntity(
        decimal amount,
        decimal canonicalAmount,
        string currency,
        int exponent,
        bool isPaidSystemMessage,
        string level
    )
    {
        Amount = amount;
        CanonicalAmount = canonicalAmount;
        Currency = currency;
        Exponent = exponent;
        IsPaidSystemMessage = isPaidSystemMessage;
        Level = level;
    }

    public static PinnedChatPaidEntity TryCreate(Tags tags)
    {
        if (tags.GetStringValue(KnownTags.PINNED_CHAT_PAID_AMOUNT) == null)
        {
            return null;
        }

        return new PinnedChatPaidEntity(tags);
    }
}
