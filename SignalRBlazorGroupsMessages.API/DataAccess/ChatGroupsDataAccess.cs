using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Helpers;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class ChatGroupsDataAccess : IChatGroupsDataAccess
    {
        private readonly ApplicationDbContext _context;
        private readonly ISerilogger _serilogger;

        public ChatGroupsDataAccess(ApplicationDbContext context, ISerilogger serilogger)
        {
            _context = context ?? throw new Exception(nameof(context));
            _serilogger = serilogger;
        }

        public async Task<List<ChatGroups>> GetPublicChatGroupsAsync()
        {
            return await _context.ChatGroups
                .Where(p => p.PrivateGroup == false)
                .ToListAsync();
        }

        public async Task<ChatGroups> GetChatGroupByIdAsync(int id)
        {
            return await _context.ChatGroups
                .SingleAsync(c => c.ChatGroupId == id);
        }

        public List<ChatGroups> GetPrivateChatGroupsByUserId(string userId)
        {
            List<ChatGroups> privateGroupList = _context.ChatGroups
                .FromSql($"exec sp_getPrivateChatGroupsForUser @UserId={userId}")
                .ToList();

            return privateGroupList;
        }

        public bool ChatGroupexists(int groupId)
        {
            return _context.ChatGroups
                .Where(c => c.ChatGroupId == groupId)
                .Any();
        }

        public async Task AddChatGroupAsync(ChatGroups chatGroup)
        {
            _context.ChatGroups.Add(chatGroup);
            await _context.SaveChangesAsync();
            _serilogger.LogNewChatGroupCreated(chatGroup);
        }

        public async Task ModifyChatGroup(ChatGroups chatGroup)
        {
            _context.ChatGroups.Update(chatGroup);
            await _context.SaveChangesAsync();
            _serilogger.LogChatGroupModified(chatGroup);
        }

        public async Task DeleteChatGroupAsync(ChatGroups chatGroup)
        {
            _context.ChatGroups.Remove(chatGroup);
            await _context.SaveChangesAsync();
            _serilogger.LogChatGroupDeleted(chatGroup);
        }

        public async Task AddUserToPrivateChatGroup(int chatGroupid, string userId)
        {
            PrivateGroupMembers privateGroupMember = new()
            {
                PrivateChatGroupId = chatGroupid,
                UserId = userId
            };

            _context.PrivateGroupsMembers.Add(privateGroupMember);
            await _context.SaveChangesAsync();
            _serilogger.LogUserAddedToPrivateChatGroup(privateGroupMember);
        }

        public async Task RemoveUserFromPrivateChatGroup(int chatGroupid, string userId)
        {
            PrivateGroupMembers privateGroupMember = _context.PrivateGroupsMembers
                .Single(p => p.PrivateChatGroupId == chatGroupid 
                    && p.UserId == userId);

            if (privateGroupMember != null)
            {
                _context.PrivateGroupsMembers.Remove(privateGroupMember);
                await _context.SaveChangesAsync();
                _serilogger.LogUserRemovedFromPrivateChatGroup(privateGroupMember);
            }
        }
    }
}
