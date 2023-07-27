using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
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

        public async Task<List<PrivateChatGroupsDto>> GetDtoListByUserIdAsync(string userId)
        {
            List<PrivateChatGroupsDto> listPrivateGroups = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getPrivateChatGroupsForUser", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add("@userId", System.Data.SqlDbType.NVarChar).Value = userId;

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            listPrivateGroups = ReturnDtoListFromReader(listPrivateGroups, reader);
            await connection.CloseAsync();

            return listPrivateGroups;
        }

        public async Task<PrivateChatGroupsDto> GetDtoByGroupIdAsync(int groupId)
        {
            PrivateChatGroupsDto dtoToReturn = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getPrivateChatGroupByGroupId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add("groupId", System.Data.SqlDbType.Int).Value = groupId;

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            dtoToReturn = ReturnDtoFromReader(dtoToReturn, reader);
            await connection.CloseAsync();

            return dtoToReturn;
        }

        public PrivateChatGroups GetByGroupname(string groupName)
        {
            return _context.PrivateChatGroups
                .Single(c => c.ChatGroupName == groupName);
        }

        public PrivateChatGroups GetByGroupId(int groupId)
        {
            return _context.PrivateChatGroups
                .Single(g => g.ChatGroupId == groupId);
        }

        public bool GroupNameTaken(string groupName)
        {
            return _context.PrivateChatGroups
                .Any(c => c.ChatGroupName == groupName);
        }

        public async Task<PrivateGroupMembers> GetPrivateGroupMemberRecord(int chatGroupid, string userId)
        {
            return await _context.PrivateGroupsMembers
                .SingleAsync(p => p.PrivateChatGroupId == chatGroupid
                    && p.UserId == userId);
        }
        public async Task<bool> AddUserToGroupAsync(PrivateGroupMembers privateGroupMember)
        {
            _context.PrivateGroupsMembers.Add(privateGroupMember);
            return await Save();
        }

        public async Task<bool> RemoveUserFromPrivateChatGroup(PrivateGroupMembers privateGroupMember)
        {
            _context.PrivateGroupsMembers.Remove(privateGroupMember);
            return await Save();
        }

        public async Task<bool> RemoveAllUsersFromGroupAsync(int groupId)
        {
            int result = await _context.PrivateGroupsMembers
                .Where(g => g.PrivateChatGroupId == groupId)
                .ExecuteDeleteAsync();

            return result >= 0;
        }

        public async Task<bool> IsUserInPrivateGroup(int groupId, string userId)
        {
            return await _context.PrivateGroupsMembers
                .Where(g => g.PrivateChatGroupId == groupId)
                .AnyAsync(u => u.UserId == userId);
        }

        public async Task<bool> AddAsync(PrivateChatGroups newGroup)
        {
            _context.PrivateChatGroups.Add(newGroup);
            return await Save();
        }

        public async Task<bool> ModifyAsync(PrivateChatGroups modifiedGroup)
        {
            _context.PrivateChatGroups.Update(modifiedGroup);
            return await Save();
        }

        public async Task<bool> DeleteAsync(PrivateChatGroups deleteGroup)
        {
            _context.PrivateChatGroups.Remove(deleteGroup);
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

        private List<PrivateChatGroupsDto> ReturnDtoListFromReader(List<PrivateChatGroupsDto> viewList, SqlDataReader reader)
        {
            while (reader.Read())
            {
                PrivateChatGroupsDto dto = new()
                {
                    ChatGroupId      = (int)reader[0],
                    ChatGroupName    = (string)reader[1],
                    GroupCreated     = (DateTime)reader[2],
                    GroupOwnerUserId = (string)reader[3],
                    UserName         = (string)reader[4]
                };
                viewList.Add(dto);
            }
            return viewList;
        }

        private PrivateChatGroupsDto ReturnDtoFromReader(PrivateChatGroupsDto dto, SqlDataReader reader)
        {
            while (reader.Read())
            {
                dto.ChatGroupId      = (int)reader[0];
                dto.ChatGroupName    = (string)reader[1];
                dto.GroupCreated     = (DateTime)reader[2];
                dto.GroupOwnerUserId = (string)reader[3];
                dto.UserName         = (string)reader[4];
            }
            return dto;
        }

        #endregion
    }
}
