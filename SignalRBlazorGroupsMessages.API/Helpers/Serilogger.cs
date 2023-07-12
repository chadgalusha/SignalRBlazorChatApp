using ChatApplicationModels;
using Serilog;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public class Serilogger : ISerilogger
    {
        #region CHAT GROUP LOGGING

        public void ChatGroupError(string errorAtMethod, Exception ex)
        {
            Log.Error($"Error with Chat Group Service: Method [{errorAtMethod}] - Error message [{ex.Message}]");
        }

        public void NewChatGroupCreated(ChatGroups chatGroup)
        {
            Log.Information($"New chat group created. Id: {chatGroup.ChatGroupId} - Name {chatGroup.ChatGroupName} - IsPrivate {chatGroup.PrivateGroup}.");
        }

        public void ChatGroupModified(ChatGroups chatGroup)
        {
            Log.Information($"Chat group modified. Id: {chatGroup.ChatGroupId} - Name {chatGroup.ChatGroupName}");
        }

        public void ChatGroupDeleted(ChatGroups chatGroup)
        {
            Log.Information($"Chat group deleted. Id: {chatGroup.ChatGroupId} - Name {chatGroup.ChatGroupName}.");
        }

        #endregion
        #region PRIVATE CHAT GROUP LOGGING

        public void UserAddedToPrivateChatGroup(PrivateGroupMembers privateGroupMember)
        {
            Log.Information($"User added to private group. Record Id: {privateGroupMember.PrivateGroupMemberId}" +
                $" - Group Id: {privateGroupMember.PrivateChatGroupId} - User Id: {privateGroupMember.UserId}");
        }

        public void UserRemovedFromPrivateChatGroup(PrivateGroupMembers privateGroupMember)
        {
            Log.Information($"User removed from private group. Record Id: {privateGroupMember.PrivateGroupMemberId}" +
                $" - Group Id: {privateGroupMember.PrivateChatGroupId} - User Id: {privateGroupMember.UserId}");
        }

        #endregion
        #region PUBLIC MESSAGE LOGGING

        public void PublicMessageError(string errorAtMethod, Exception ex)
        {
            Log.Error($"Error with Public Message Service: Method [{errorAtMethod}] - Error message [{ex.Message}]");
        }

        public void PublicMessageModified(PublicMessages message)
        {
            Log.Information($"Public message modified. Id: {message.PublicMessageId} - UserId: {message.UserId} - Chat Group: {message.ChatGroupId}.");
        }

        public void PublicMessageDeleted(PublicMessages message)
        {
            Log.Information($"Public message deleted. Id: {message.PublicMessageId} - UserId: {message.UserId} - Chat Group: {message.ChatGroupId}.");
        }

        #endregion
        #region PRIVATE MESSAGE LOGGING

        public void PrivateMessageError(string errorAtMethod, Exception ex)
        {
            Log.Error($"Error with Private Message Service: Method [{errorAtMethod}] - Error message [{ex.Message}]");
        }

        public void PrivateMessageModified(PrivateMessages message)
        {
            Log.Information($"Private message modified. Id: {message.PrivateMessageId} - From UserId: {message.FromUserId} - To UserId: {message.ToUserId}.");
        }

        public void PrivateMessageDeleted(PrivateMessages message)
        {
            Log.Information($"Private message deleted. Id: {message.PrivateMessageId} - From UserId: {message.FromUserId} - To UserId: {message.ToUserId}.");
        }

        #endregion
        #region CONTROLLER LOGGING

        public void GetRequest<T>(string ipv4, ApiResponse<T> apiResponse)
        {
            Log.Information($"[{ipv4}] Get request. Api Response: Message - [{apiResponse.Message}] | Success - [{apiResponse.Success}] | Data type - [{TypeString(apiResponse)}]");
        }

        public void PostRequest<T>(string ipv4, ApiResponse<T> apiResponse)
        {
            Log.Information($"[{ipv4}] Post request. Api Response: Message - [{apiResponse.Message}] | Success - [{apiResponse.Success}] | Data type - [{TypeString(apiResponse)}]");
        }

        public void PutRequest<T>(string ipv4, ApiResponse<T> apiResponse)
        {
            Log.Information($"[{ipv4}] Put request. Api Response: Message - [{apiResponse.Message}] | Success - [{apiResponse.Success}] | Data type - [{TypeString(apiResponse)}]");
        }

        public void DeleteRequest<T>(string ipv4, ApiResponse<T> apiResponse)
        {
            Log.Information($"[{ipv4}] Delete request. Api Response: Message - [{apiResponse.Message}] | Success - [{apiResponse.Success}] | Data type - [{TypeString(apiResponse)}]");
        }

        #endregion

        #region PRIVATE METHODS

        private string TypeString<T>(ApiResponse<T> apiResponse)
        {
            if (apiResponse.Data == null)
                return "no data";
            return apiResponse.Data!.GetType().ToString();
        }

        #endregion
    }
}
