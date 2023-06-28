using ChatApplicationModels;
using Serilog;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public class Serilogger : ISerilogger
    {
        #region CHAT GROUP LOGGING

        public void LogNewChatGroupCreated(ChatGroups chatGroup)
        {
            Log.Information($"New chat group created. Id: {chatGroup.ChatGroupId} - Name {chatGroup.ChatGroupName} - IsPrivate {chatGroup.PrivateGroup}.");
        }

        public void LogChatGroupModified(ChatGroups chatGroup)
        {
            Log.Information($"Chat group modified. Id: {chatGroup.ChatGroupId} - Name {chatGroup.ChatGroupName}");
        }

        public void LogChatGroupDeleted(ChatGroups chatGroup)
        {
            Log.Information($"Chat group deleted. Id: {chatGroup.ChatGroupId} - Name {chatGroup.ChatGroupName}.");
        }

        #endregion
        #region PRIVATE CHAT GROUP LOGGING

        public void LogUserAddedToPrivateChatGroup(PrivateGroupMembers privateGroupMember)
        {
            Log.Information($"User added to private group. Record Id: {privateGroupMember.PrivateGroupMemberId}" +
                $" - Group Id: {privateGroupMember.PrivateChatGroupId} - User Id: {privateGroupMember.UserId}");
        }

        public void LogUserRemovedFromPrivateChatGroup(PrivateGroupMembers privateGroupMember)
        {
            Log.Information($"User removed from private group. Record Id: {privateGroupMember.PrivateGroupMemberId}" +
                $" - Group Id: {privateGroupMember.PrivateChatGroupId} - User Id: {privateGroupMember.UserId}");
        }

        #endregion
        #region PUBLIC MESSAGE LOGGING

        public void LogPublicMessageModified(PublicMessages message)
        {
            Log.Information($"Public message modified. Id: {message.PublicMessageId} - UserId: {message.UserId} - Chat Group: {message.ChatGroupId}.");
        }

        public void LogPublicMessageDeleted(PublicMessages message)
        {
            Log.Information($"Public message deleted. Id: {message.PublicMessageId} - UserId: {message.UserId} - Chat Group: {message.ChatGroupId}.");
        }

        #endregion
        #region PRIVATE MESSAGE LOGGING

        public void LogPrivateMessageModified(PrivateMessages message)
        {
            Log.Information($"Private message modified. Id: {message.PrivateMessageId} - From UserId: {message.FromUserId} - To UserId: {message.ToUserId}.");
        }

        public void LogPrivateMessageDeleted(PrivateMessages message)
        {
            Log.Information($"Private message deleted. Id: {message.PrivateMessageId} - From UserId: {message.FromUserId} - To UserId: {message.ToUserId}.");
        }

        #endregion
    }
}
