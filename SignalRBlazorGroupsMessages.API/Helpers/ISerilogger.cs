using ChatApplicationModels;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public interface ISerilogger
    {
        void LogChatGroupDeleted(ChatGroups chatGroup);
        void LogChatGroupModified(ChatGroups chatGroup);
        void LogNewChatGroupCreated(ChatGroups chatGroup);
        void LogPrivateMessageDeleted(PrivateMessages message);
        void LogPrivateMessageModified(PrivateMessages message);
        void LogPublicMessageDeleted(PublicMessages message);
        void LogPublicMessageModified(PublicMessages message);
        void LogUserAddedToPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        void LogUserRemovedFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
    }
}