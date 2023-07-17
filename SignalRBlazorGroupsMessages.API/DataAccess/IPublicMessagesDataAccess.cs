﻿using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Models.Views;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public interface IPublicMessagesDataAccess
    {
        Task<bool> AddAsync(PublicGroupMessages message);
        Task<bool> DeleteAsync(PublicGroupMessages message);
        Task<List<PublicGroupMessagesView>> GetViewListByGroupIdAsync(int groupId, int numberItemsToSkip);
        Task<List<PublicGroupMessagesView>> GetViewListByUserIdAsync(Guid userId, int numberItemsToSkip);
        Task<PublicGroupMessagesView> GetViewByMessageIdAsync(Guid messageId);
        Task<bool> ModifyAsync(PublicGroupMessages message);
        Task<bool> Exists(Guid messageId);
        Task<PublicGroupMessages> GetByMessageIdAsync(Guid messageId);
        Task<bool> DeleteMessagesByResponseMessageIdAsync(Guid responseMessageId);
        Task<bool> DeleteAllMessagesInGroupAsync(int chatGroupId);
    }
}