using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class ChatGroupsDataAccess : IChatGroupsDataAccess
    {
        private readonly ApplicationDbContext _context;

        public ChatGroupsDataAccess(ApplicationDbContext context)
        {
            _context = context ?? throw new Exception(nameof(context));
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
                .Any(c => c.ChatGroupId == groupId);
        }

        public async Task<bool> IsPublicChatGroup(int groupId)
        {
            return await _context.ChatGroups
                .Where(c => c.PrivateGroup == true)
                .Where(c => c.ChatGroupId == groupId)
                .AnyAsync();
        }

        public async Task<bool> AddChatGroupAsync(ChatGroups chatGroup)
        {
            _context.ChatGroups.Add(chatGroup);
            return await Save();
        }

        public async Task<bool> ModifyChatGroupAsync(ChatGroups chatGroup)
        {
            _context.ChatGroups.Update(chatGroup);
            return await Save();
        }

        public async Task<bool> DeleteChatGroupAsync(ChatGroups chatGroup)
        {
            _context.ChatGroups.Remove(chatGroup);
            return await Save();
        }

        public async Task<bool> AddUserToPrivateChatGroupAsync(PrivateGroupMembers privateGroupMember)
        {
            _context.PrivateGroupsMembers.Add(privateGroupMember);
            return await Save();
        }

        public async Task<PrivateGroupMembers> GetPrivateGroupMemberRecord(int chatGroupid, string userId)
        {
            return await _context.PrivateGroupsMembers
                .SingleAsync(p => p.PrivateChatGroupId == chatGroupid
                    && p.UserId == userId);
        }

        public async Task<bool> RemoveUserFromPrivateChatGroup(PrivateGroupMembers privateGroupMember)
        {
            _context.PrivateGroupsMembers.Remove(privateGroupMember);
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
