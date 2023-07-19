using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models.Views;
using System.Data.SqlClient;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class PrivateChatGroupsDataAccess : IPrivateChatGroupsDataAccess
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PrivateChatGroupsDataAccess(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new Exception(nameof(context));
            _configuration = configuration ?? throw new Exception(nameof(configuration));
        }

        public async Task<List<PrivateChatGroupsView>> GetViewListPrivateByUserIdAsync(Guid userId)
        {
            List<PrivateChatGroupsView> listPrivateGroups = new();

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

        private List<PrivateChatGroupsView> ReturnViewListFromReader(List<PrivateChatGroupsView> viewList, SqlDataReader reader)
        {
            while (reader.Read())
            {
                PrivateChatGroupsView view = new()
                {
                    ChatGroupId = (int)reader[0],
                    ChatGroupName = (string)reader[1],
                    GroupCreated = (DateTime)reader[2],
                    GroupOwnerUserId = Guid.Parse((string)reader[3]),
                    UserName = (string)reader[4]
                };
                viewList.Add(view);
            }
            return viewList;
        }

        private PrivateChatGroupsView ReturnViewFromReader(PrivateChatGroupsView view, SqlDataReader reader)
        {
            while (reader.Read())
            {
                view.ChatGroupId = (int)reader[0];
                view.ChatGroupName = (string)reader[1];
                view.GroupCreated = (DateTime)reader[2];
                view.GroupOwnerUserId = Guid.Parse((string)reader[3]);
                view.UserName = (string)reader[4];
            }
            return view;
        }

        #endregion
    }
}
