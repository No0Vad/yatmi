using Yatmi.Enum;

namespace Yatmi.Entities;

public class ReplyThreadEntity
{
    /// <summary>
    /// The message ID being replied to
    /// </summary>
    public string ReplyID { get; }

    /// <summary>
    /// The Username of the message being replied to
    /// </summary>
    public string ReplyUsername { get; }

    /// <summary>
    /// The UserID of the message being replied to
    /// </summary>
    public string ReplyUserID { get; }

    /// <summary>
    /// The message body of the message being replied to
    /// </summary>
    public string ReplyBody { get; }


    /// <summary>
    /// The first message ID that was replied too, creating a thread
    /// </summary>
    public string ThreadID { get; }

    /// <summary>
    /// The first Username that was replied too in the thread
    /// </summary>
    public string ThreadUsername { get; }

    /// <summary>
    /// The first UsernID that was replied too in the thread
    /// </summary>
    public string ThreadUserID { get; }

    public ReplyThreadEntity(Tags tags)
    {
        ReplyID = tags.GetStringValue(KnownTags.REPLY_PARENT_MSG_ID);
        ReplyUsername = tags.GetStringValue(KnownTags.REPLY_PARENT_MSG_USER_LOGIN);
        ReplyUserID = tags.GetStringValue(KnownTags.REPLY_PARENT_MSG_USER_ID);
        ReplyBody = tags.GetStringValue(KnownTags.REPLY_PARENT_MSG_BODY);

        ThreadID = tags.GetStringValue(KnownTags.REPLY_THREAD_PARENT_MSG_ID);
        ThreadUsername = tags.GetStringValue(KnownTags.REPLY_THREAD_PARENT_USER_LOGIN);
        ThreadUserID = tags.GetStringValue(KnownTags.REPLY_THREAD_PARENT_USER_ID);
    }

    public ReplyThreadEntity(
        string replyId,
        string replyUsername,
        string replyUserId,
        string replyBody,
        string threadId,
        string threadUsername,
        string threadUserId
    )
    {
        ReplyID = replyId;
        ReplyUsername = replyUsername;
        ReplyUserID = replyUserId;
        ReplyBody = replyBody;

        ThreadID = threadId;
        ThreadUsername = threadUsername;
        ThreadUserID = threadUserId;
    }

    public static ReplyThreadEntity TryCreate(Tags tags)
    {
        if (tags.GetStringValue(KnownTags.REPLY_PARENT_MSG_ID) == null)
        {
            return null;
        }

        return new ReplyThreadEntity(tags);
    }
}