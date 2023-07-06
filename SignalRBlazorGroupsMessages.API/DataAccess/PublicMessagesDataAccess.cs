using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class PublicMessagesDataAccess : IPublicMessagesDataAccess
    {
        private readonly ApplicationDbContext _context;

        public PublicMessagesDataAccess(ApplicationDbContext context)
        {
            _context = context ?? throw new Exception(nameof(context));
        }

        public async Task<List<PublicMessagesView>> GetViewListByGroupIdAsync(int groupId, int numberItemsToSkip)
        {
            return await _context.Database
                .SqlQuery<PublicMessagesView>($"EXECUTE sp_getPublicMessages_byGroupId @groupId={groupId}, @numberMessagesToSkip={numberItemsToSkip}")
                .ToListAsync();
        }

        public async Task<List<PublicMessagesView>> GetViewListByUserIdAsync(Guid userId, int numberItemsToSkip)
        {
            return await _context.Database
                .SqlQuery<PublicMessagesView>($"EXECUTE sp_getPublicMessage_byMessageId @userId={userId}, @numberMessagesToSkip={numberItemsToSkip}")
                .ToListAsync();
        }

        public async Task<PublicMessagesView> GetViewByMessageIdAsync(Guid messageId)
        {
            return await _context.Database
                .SqlQuery<PublicMessagesView>($"EXECUTE sp_getPublicMessage_byMessageId @messageId={messageId}")
                .SingleAsync();
        }

        public async Task<PublicMessages> GetByMessageIdAsync(Guid messageId)
        {
            return await _context.PublicMessages.SingleAsync(p => p.PublicMessageId == messageId);
        }

        public async Task<bool> Exists(Guid messageId)
        {
            return await _context.PublicMessages
                .AnyAsync(p => p.PublicMessageId == messageId);
        }

        public async Task<bool> AddAsync(PublicMessages message)
        {
            await _context.PublicMessages.AddAsync(message);
            return await Save();
        }

        public async Task<bool> ModifyAsync(PublicMessages message)
        {
            return await Save();
        }

        public async Task<bool> DeleteAsync(PublicMessages message)
        {
            _context.PublicMessages.Remove(message);
            return await Save();
        }

        public async Task<bool> DeleteMessagesByResponseMessageIdAsync(Guid responseMessageId)
        {
            int result = await _context.PublicMessages
                .Where(r => r.ReplyMessageId == responseMessageId)
                .ExecuteDeleteAsync();

            return result >= 0;
        }

        #region PRIVATE METHODS

        private async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        #endregion
    }
}
