using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public interface ISerilogger
    {
        void LogChatGroupError(string errorAtMethod, Exception ex);
        void LogChatGroupDeleted(ChatGroups chatGroup);
        void LogChatGroupModified(ChatGroups chatGroup);
        void LogNewChatGroupCreated(ChatGroups chatGroup);
        void LogPrivateMessageError(string errorAtMethod, Exception ex);
        void LogPrivateMessageDeleted(PrivateMessages message);
        void LogPrivateMessageModified(PrivateMessages message);
        void LogPublicMessageError(string errorAtMethod, Exception ex);
        void LogPublicMessageDeleted(PublicMessages message);
        void LogPublicMessageModified(PublicMessages message);
        void LogUserAddedToPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        void LogUserRemovedFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
    }
}