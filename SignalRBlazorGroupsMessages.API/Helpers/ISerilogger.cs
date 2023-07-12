using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public interface ISerilogger
    {
        void ChatGroupError(string errorAtMethod, Exception ex);
        void ChatGroupDeleted(ChatGroups chatGroup);
        void ChatGroupModified(ChatGroups chatGroup);
        void NewChatGroupCreated(ChatGroups chatGroup);
        void PrivateMessageError(string errorAtMethod, Exception ex);
        void PrivateMessageDeleted(PrivateMessages message);
        void PrivateMessageModified(PrivateMessages message);
        void PublicMessageError(string errorAtMethod, Exception ex);
        void PublicMessageDeleted(PublicMessages message);
        void PublicMessageModified(PublicMessages message);
        void UserAddedToPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        void UserRemovedFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        void GetRequest<T>(string ipv4, ApiResponse<T> apiResponse);
        void PostRequest<T>(string ipv4, ApiResponse<T> apiResponse);
        void PutRequest<T>(string ipv4, ApiResponse<T> apiResponse);
        void DeleteRequest<T>(string ipv4, ApiResponse<T> apiResponse);
    }
}