using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class PublicMessagesDataAccess : IPublicMessagesDataAccess
    {
        private readonly ApplicationDbContext _context;

        public PublicMessagesDataAccess(ApplicationDbContext context)
        {
            _context = context ?? throw new Exception(nameof(context));
        }

        public async Task<List<PublicMessages>> GetMessagesByGroupIdAsync(int groupId, int currentItemCount)
        {
            return await _context.PublicMessages
                .Where(g => g.ChatGroupId == groupId)
                .OrderByDescending(o => o.MessageDateTime)
                .Skip(currentItemCount)
                .Take(50)
                .ToListAsync();
        }

        public async Task<List<PublicMessages>> GetMessagesByUserIdAsync(string userId, int currentItemCount)
        {
            return await _context.PublicMessages
                .Where(u => u.UserId == userId)
                .OrderByDescending(o => o.MessageDateTime)
                .Skip(currentItemCount)
                .Take(50)
                .ToListAsync();
        }

        public async Task<PublicMessages> GetPublicMessageByIdAsync(string messageId)
        {
            return await _context.PublicMessages
                .SingleAsync(p => p.PublicMessageId == messageId);
        }

        public async Task<bool> PublicMessageExists(string messageId)
        {
            return await _context.PublicMessages
                .AnyAsync(p => p.PublicMessageId == messageId);
        }

        public async Task<bool> AddMessageAsync(PublicMessages message)
        {
            await _context.PublicMessages.AddAsync(message);
            return await Save();
        }

        public async Task<bool> ModifyMessageAsync(PublicMessages message)
        {
            return await Save();
        }

        public async Task<bool> DeleteMessage(PublicMessages message)
        {
            _context.PublicMessages.Remove(message);
            return await Save();
        }

        #region PRIVATE METHODS

        private async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        #endregion
    }
}
