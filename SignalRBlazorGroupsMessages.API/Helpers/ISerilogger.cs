using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.Helpers
{
    public interface ISerilogger
    {
        void ChatGroupError(string errorAtMethod, Exception ex);
        void ChatGroupDeleted(PublicChatGroups chatGroup);
        void ChatGroupModified(PublicChatGroups chatGroup);
        void NewChatGroupCreated(PublicChatGroups chatGroup);
        void PrivateMessageError(string errorAtMethod, Exception ex);
        void PrivateMessageDeleted(PrivateUserMessages message);
        void PrivateMessageModified(PrivateUserMessages message);
        void PublicMessageError(string errorAtMethod, Exception ex);
        void PublicMessageDeleted(PublicGroupMessages message);
        void PublicMessageModified(PublicGroupMessages message);
        void UserAddedToPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        void UserRemovedFromPrivateChatGroup(PrivateGroupMembers privateGroupMember);
        void GetRequest<T>(string ipv4, ApiResponse<T> apiResponse);
        void PostRequest<T>(string ipv4, ApiResponse<T> apiResponse);
        void PutRequest<T>(string ipv4, ApiResponse<T> apiResponse);
        void DeleteRequest<T>(string ipv4, ApiResponse<T> apiResponse);
    }
}