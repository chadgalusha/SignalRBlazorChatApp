using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models;

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
        void PublicMessageError(string errorAtMethod, Exception ex);
        void PublicMessageDeleted(PublicMessages message);
        void PublicMessageModified(PublicMessages message);
        void LogUserAddedToPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        void LogUserRemovedFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        void GetRequest<T>(string ipv4, ApiResponse<T> apiResponse);
        void PostRequest<T>(string ipv4, ApiResponse<T> apiResponse);
        void PutRequest<T>(string ipv4, ApiResponse<T> apiResponse);
        void DeleteRequest<T>(string ipv4, ApiResponse<T> apiResponse);
    }
}