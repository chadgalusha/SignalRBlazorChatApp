using ChatApplicationModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class ChatGroupsDataAccess : IChatGroupsDataAccess
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ChatGroupsDataAccess(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new Exception(nameof(context));
            _configuration = configuration ?? throw new Exception(nameof(configuration));
        }

        public async Task<List<ChatGroupsView>> GetViewListPublicChatGroupsAsync()
        {
            List<ChatGroupsView> viewList = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getPublicChatGroups", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            viewList = ReturnViewListFromReader(viewList, reader);
            await connection.CloseAsync();

            return viewList;
        }

        public async Task<ChatGroupsView> GetChatGroupByIdAsync(int groupId)
        {
            ChatGroupsView view = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getChatGroup_byGroupId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add("@groupId", System.Data.SqlDbType.Int).Value = groupId;

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            view = ReturnViewFromReader(view, reader);
            await connection.CloseAsync();

            return view;
        }

        public async Task<List<ChatGroupsView>> GetViewListPrivateByUserIdAsync(Guid userId)
        {
            List<ChatGroupsView> listPrivateGroups = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getPrivateChatGroupsForUser", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add("@userId", System.Data.SqlDbType.NVarChar).Value = userId.ToString();

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            listPrivateGroups = ReturnViewListFromReader(listPrivateGroups, reader);
            await connection.CloseAsync();

            return listPrivateGroups;
        }

        public PublicChatGroups GetByGroupName(string chatGroupName)
        {
            return _context.ChatGroups
                .Single(c => c.ChatGroupName == chatGroupName);
        }

        public PublicChatGroups GetByGroupId(int groupId)
        {
            return _context.ChatGroups
                .Single(c => c.ChatGroupId == groupId);
        }

        public bool GroupNameTaken(string chatGroupName)
        {
            return _context.ChatGroups
                .Any(c => c.ChatGroupName == chatGroupName);
        }

        public bool GroupExists(int groupId)
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

        public async Task<bool> AddAsync(PublicChatGroups chatGroup)
        {
            _context.ChatGroups.Add(chatGroup);
            return await Save();
        }

        public async Task<bool> ModifyAsync(PublicChatGroups chatGroup)
        {
            _context.ChatGroups.Update(chatGroup);
            return await Save();
        }

        public async Task<bool> DeleteAsync(PublicChatGroups chatGroup)
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

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("ChatApplicationDb")!;
        }

        private List<ChatGroupsView> ReturnViewListFromReader(List<ChatGroupsView> viewList, SqlDataReader reader)
        {
            while (reader.Read())
            {
                ChatGroupsView view = new()
                {
                    ChatGroupId      = (int)reader[0],
                    ChatGroupName    = (string)reader[1],
                    GroupCreated     = (DateTime)reader[2],
                    GroupOwnerUserId = Guid.Parse((string)reader[3]),
                    UserName         = (string)reader[4],
                    PrivateGroup     = (bool)reader[5]
                };
                viewList.Add(view);
            }
            return viewList;
        }

        private ChatGroupsView ReturnViewFromReader(ChatGroupsView view, SqlDataReader reader)
        {
            while (reader.Read())
            {
                view.ChatGroupId      = (int)reader[0];
                view.ChatGroupName    = (string)reader[1];
                view.GroupCreated     = (DateTime)reader[2];
                view.GroupOwnerUserId = Guid.Parse((string)reader[3]);
                view.UserName         = (string)reader[4];
                view.PrivateGroup     = (bool)reader[5];
            }
            return view;
        }

        #endregion
    }
}
