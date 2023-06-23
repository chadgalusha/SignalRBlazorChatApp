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

        public async Task<List<PublicMessages>> GetMessagesByGroupAsync(int groupId)
        {
            List<PublicMessages> messages = new();
            messages = await _context.PublicMessages
                .Where(g => g.ChatGroupId == groupId)
                .OrderByDescending(o => o.MessageDateTime)
                .Take(50)
                .ToListAsync();

            return messages;
        }

        public async Task<PublicMessages> GetPublicMessageByIdAsync(string messageId)
        {
            return await _context.PublicMessages
                .Where(p => p.PublicMessageId == messageId)
                .FirstAsync();
        }

        public async Task<bool> PublicMessageExists(string messageId)
        {
            return await _context.PublicMessages
                .Where(p => p.PublicMessageId == messageId)
                .AnyAsync();
        }

        public async Task AddMessageAsync(PublicMessages message)
        {
            await _context.PublicMessages.AddAsync(message);
        }

        public async Task ModifyMessageAsync(PublicMessages message)
        {
            _context.PublicMessages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMessage(PublicMessages message)
        {
            _context.PublicMessages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }
}
