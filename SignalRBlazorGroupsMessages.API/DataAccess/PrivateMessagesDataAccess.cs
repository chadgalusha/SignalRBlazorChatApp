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

        public async Task<List<PrivateUserMessages>> GetAllPrivateMessagesForUserAsync(string userId)
        {
            return await _context.PrivateMessages
                .Where(p => p.ToUserId == userId)
                .ToListAsync();
        }

        public async Task<List<PrivateUserMessages>> GetPrivateMessagesFromUserAsync(string toUserId, string fromUserId)
        {
            return await _context.PrivateMessages
                .Where(p => p.ToUserId == toUserId)
                .Where(p => p.FromUserId == fromUserId)
                .OrderByDescending(p => p.PrivateMessageDateTime)
                .ToListAsync();
        }

        public PrivateUserMessages GetPrivateMessage(int privateMessageId)
        {
            PrivateUserMessages? message = _context.PrivateMessages.SingleOrDefault(p => p.PrivateMessageId == privateMessageId);

            return message ?? 
                throw new GroupsMessagesExceptions($"Private message with id of {privateMessageId} not found.");
        }

        public async Task<bool> AddPrivateMessageAsync(PrivateUserMessages privateMessage)
        {
            _context.PrivateMessages.Add(privateMessage);
            return await Save();
        }

        public async Task<bool> ModifyPrivateMessageAsync(PrivateUserMessages privateMessage)
        {
            _context.PrivateMessages.Update(privateMessage);
            return await Save();
        }

        public async Task<bool> DeletePrivateMessageAsync(PrivateUserMessages privateMessage)
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
