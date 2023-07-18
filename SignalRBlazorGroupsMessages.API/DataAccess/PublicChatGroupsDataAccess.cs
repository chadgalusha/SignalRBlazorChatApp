using ChatApplicationModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class PublicChatGroupsDataAccess : IPublicChatGroupsDataAccess
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PublicChatGroupsDataAccess(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new Exception(nameof(context));
            _configuration = configuration ?? throw new Exception(nameof(configuration));
        }

        public async Task<List<PublicChatGroupsDto>> GetDtoListAsync()
        {
            List<PublicChatGroupsDto> dtoList = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getPublicChatGroups", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            dtoList = ReturnViewListFromReader(dtoList, reader);
            await connection.CloseAsync();

            return dtoList;
        }

        public async Task<PublicChatGroupsDto> GetDtoByIdAsync(int groupId)
        {
            PublicChatGroupsDto view = new();

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

        public PublicChatGroups GetByGroupName(string chatGroupName)
        {
            return _context.PublicChatGroups
                .Single(c => c.ChatGroupName == chatGroupName);
        }

        public PublicChatGroups GetByGroupId(int groupId)
        {
            return _context.PublicChatGroups
                .Single(c => c.ChatGroupId == groupId);
        }

        public bool GroupNameTaken(string chatGroupName)
        {
            return _context.PublicChatGroups
                .Any(c => c.ChatGroupName == chatGroupName);
        }

        public bool GroupExists(int groupId)
        {
            return _context.PublicChatGroups
                .Any(c => c.ChatGroupId == groupId);
        }

        public async Task<bool> AddAsync(PublicChatGroups chatGroup)
        {
            _context.PublicChatGroups.Add(chatGroup);
            return await Save();
        }

        public async Task<bool> ModifyAsync(PublicChatGroups chatGroup)
        {
            _context.PublicChatGroups.Update(chatGroup);
            return await Save();
        }

        public async Task<bool> DeleteAsync(PublicChatGroups chatGroup)
        {
            _context.PublicChatGroups.Remove(chatGroup);
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

        private List<PublicChatGroupsDto> ReturnViewListFromReader(List<PublicChatGroupsDto> dtoList, SqlDataReader reader)
        {
            while (reader.Read())
            {
                PublicChatGroupsDto dto = new()
                {
                    ChatGroupId      = (int)reader[0],
                    ChatGroupName    = (string)reader[1],
                    GroupCreated     = (DateTime)reader[2],
                    GroupOwnerUserId = Guid.Parse((string)reader[3]),
                    UserName         = (string)reader[4]
                };
                dtoList.Add(dto);
            }
            return dtoList;
        }

        private PublicChatGroupsDto ReturnViewFromReader(PublicChatGroupsDto dto, SqlDataReader reader)
        {
            while (reader.Read())
            {
                dto.ChatGroupId      = (int)reader[0];
                dto.ChatGroupName    = (string)reader[1];
                dto.GroupCreated     = (DateTime)reader[2];
                dto.GroupOwnerUserId = Guid.Parse((string)reader[3]);
                dto.UserName         = (string)reader[4];
            }
            return dto;
        }

        #endregion
    }
}
