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

        public List<ChatGroups> GetPrivateChatGroupsByUserId(string userId)
        {
            List<PrivateGroupMembers> privateGroupMemberList = GetListPrivateGroupMembersForUser(userId);
            List<ChatGroups> privateGroupList = GetListPrivateChatGroups(privateGroupMemberList);

            return privateGroupList;
        }

        public async Task<bool> ChatGroupexists(int groupId)
        {
            return await _context.ChatGroups
                .Where(c => c.ChatGroupId == groupId)
                .AnyAsync();
        }

        public async Task AddChatGroupAsync(ChatGroups chatGroup)
        {
            _context.ChatGroups.Add(chatGroup);
            await _context.SaveChangesAsync();
        }

        public async Task ModifyChatGroup(ChatGroups chatGroup)
        {
            _context.ChatGroups.Update(chatGroup);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteChatGroupAsync(ChatGroups chatGroup)
        {
            _context.ChatGroups.Remove(chatGroup);
            await _context.SaveChangesAsync();
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
        }

        public async Task RemoveUserFromPrivateChatGroup(int chatGroupid, string userId)
        {
            PrivateGroupMembers privateGroupMember = _context.PrivateGroupsMembers
                .Where(p => p.PrivateChatGroupId == chatGroupid)
                .Where(p => p.UserId == userId)
                .First();

            if (privateGroupMember != null)
            {
                _context.PrivateGroupsMembers.Remove(privateGroupMember);
                await _context.SaveChangesAsync();
            }
        }

        #region PRIVATE METHODS

        private List<PrivateGroupMembers> GetListPrivateGroupMembersForUser(string userid)
        {
            return _context.PrivateGroupsMembers
                .Where(p => p.UserId == userid)
                .ToList();
        }

        private List<ChatGroups> GetListPrivateChatGroups(List<PrivateGroupMembers> privateGroupMemberList)
        {
            List<ChatGroups> chatGroupsList = new();

            foreach (var item in privateGroupMemberList)
            {
                ChatGroups group = _context.ChatGroups
                    .Where(c => c.ChatGroupId == item.PrivateChatGroupId)
                    .First();

                if (group != null)
                {
                    chatGroupsList.Add(group);
                }
            }

            return chatGroupsList;
        }
        #endregion
    }
}
