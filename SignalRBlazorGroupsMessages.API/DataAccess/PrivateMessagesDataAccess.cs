using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Helpers;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class PrivateMessagesDataAccess : IPrivateMessagesDataAccess
    {
        private readonly ApplicationDbContext _context;

        public PrivateMessagesDataAccess(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PrivateMessages>> GetAllPrivateMessagesForUserAsync(string userId)
        {
            return await _context.PrivateMessages
                .Where(p => p.ToUserId == userId)
                .ToListAsync();
        }

        public async Task<List<PrivateMessages>> GetPrivateMessagesFromUserAsync(string toUserId, string fromUserId)
        {
            return await _context.PrivateMessages
                .Where(p => p.ToUserId == toUserId)
                .Where(p => p.FromUserId == fromUserId)
                .OrderByDescending(p => p.PrivateMessageDateTime)
                .ToListAsync();
        }

        public PrivateMessages GetPrivateMessage(int privateMessageId)
        {
            PrivateMessages? message = _context.PrivateMessages.SingleOrDefault(p => p.PrivateMessageId == privateMessageId);

            return message ?? 
                throw new GroupsMessagesExceptions($"Private message with id of {privateMessageId} not found.");
        }

        public async Task<bool> AddPrivateMessageAsync(PrivateMessages privateMessage)
        {
            _context.PrivateMessages.Add(privateMessage);
            return await Save();
        }

        public async Task<bool> ModifyPrivateMessageAsync(PrivateMessages privateMessage)
        {
            _context.PrivateMessages.Update(privateMessage);
            return await Save();
        }

        public async Task<bool> DeletePrivateMessageAsync(PrivateMessages privateMessage)
        {
            _context.PrivateMessages.Remove(privateMessage);
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
