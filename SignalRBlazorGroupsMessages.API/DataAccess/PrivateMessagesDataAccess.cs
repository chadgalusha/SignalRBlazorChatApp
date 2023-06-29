using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Helpers;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class PrivateMessagesDataAccess : IPrivateMessagesDataAccess
    {
        private readonly ApplicationDbContext _context;
        private readonly ISerilogger _serilogger;

        public PrivateMessagesDataAccess(ApplicationDbContext context, ISerilogger serilogger)
        {
            _context = context;
            _serilogger = serilogger;
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
            bool messageExists = _context.PrivateMessages.Any(p => p.PrivateMessageId == privateMessageId);

            if (!messageExists)
            {
                return new() { PrivateMessageId = -1 };
            }
            return _context.PrivateMessages.Single(p => p.PrivateMessageId == privateMessageId);
        }

        public async Task AddPrivateMessageAsync(PrivateMessages privateMessage)
        {
            _context.PrivateMessages.Add(privateMessage);
            await _context.SaveChangesAsync();
        }

        public async Task ModifyPrivateMessageAsync(PrivateMessages privateMessage)
        {
            _context.PrivateMessages.Update(privateMessage);
            await _context.SaveChangesAsync();
            _serilogger.LogPrivateMessageModified(privateMessage);
        }

        public async Task DeletePrivateMessageAsync(PrivateMessages privateMessage)
        {
            _context.PrivateMessages.Remove(privateMessage);
            await _context.SaveChangesAsync();
            _serilogger.LogPrivateMessageDeleted(privateMessage);
        }
    }
}
