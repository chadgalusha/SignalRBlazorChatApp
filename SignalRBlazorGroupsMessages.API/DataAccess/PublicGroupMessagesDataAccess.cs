using ChatApplicationModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models.Dtos;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class PublicGroupMessagesDataAccess : IPublicGroupMessagesDataAccess
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PublicGroupMessagesDataAccess(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new Exception(nameof(context));
            _configuration = configuration ?? throw new Exception(nameof(configuration));
        }

        public async Task<List<PublicGroupMessageDto>> GetDtoListByGroupIdAsync(int groupId, int numberItemsToSkip)
        {
            List<PublicGroupMessageDto> dtoList = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getPublicMessages_byGroupId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add("@groupId", System.Data.SqlDbType.Int).Value = groupId;
            command.Parameters.Add("@numberMessagesToSkip", System.Data.SqlDbType.Int).Value = numberItemsToSkip;

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            dtoList = ReturnDtoListFromReader(dtoList, reader);
            await connection.CloseAsync();

            return dtoList;
        }

        public async Task<List<PublicGroupMessageDto>> GetDtoListByUserIdAsync(string userId, int numberItemsToSkip)
        {
            List<PublicGroupMessageDto> dtoList = new();

            using SqlConnection connection= new(GetConnectionString());
            SqlCommand command = new("sp_getPublicMessages_byUserId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add("@userId", System.Data.SqlDbType.NVarChar).Value = userId;
            command.Parameters.Add("@numberMessagesToSkip", System.Data.SqlDbType.Int).Value = numberItemsToSkip;

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            dtoList = ReturnDtoListFromReader(dtoList, reader);
            await connection.CloseAsync();

            return dtoList;
        }

        public async Task<PublicGroupMessageDto> GetDtoByMessageIdAsync(Guid messageId)
        {
            PublicGroupMessageDto dto = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getPublicMessage_byMessageId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add("@messageId", System.Data.SqlDbType.UniqueIdentifier).Value = messageId;

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            dto = ReturnDtoFromReader(dto, reader);
            await connection.CloseAsync();

            return dto;
        }

        public async Task<PublicGroupMessages> GetByMessageIdAsync(Guid messageId)
        {
            return await _context.PublicGroupMessages
                .SingleAsync(p => p.PublicMessageId == messageId);
        }

        public async Task<bool> Exists(Guid messageId)
        {
            return await _context.PublicGroupMessages
                .AnyAsync(p => p.PublicMessageId == messageId);
        }

        public async Task<bool> AddAsync(PublicGroupMessages message)
        {
            await _context.PublicGroupMessages.AddAsync(message);
            return await Save();
        }

        public async Task<bool> ModifyAsync(PublicGroupMessages message)
        {
            return await Save();
        }

        public async Task<bool> DeleteAsync(PublicGroupMessages message)
        {
            _context.PublicGroupMessages.Remove(message);
            return await Save();
        }

        public async Task<bool> DeleteMessagesByResponseMessageIdAsync(Guid responseMessageId)
        {
            int result = await _context.PublicGroupMessages
                .Where(r => r.ReplyMessageId == responseMessageId)
                .ExecuteDeleteAsync();

            return result >= 0;
        }

        public async Task<bool> DeleteAllMessagesInGroupAsync(int chatGroupId)
        {
            int result = await _context.PublicGroupMessages
                .Where(c => c.ChatGroupId == chatGroupId)
                .ExecuteDeleteAsync();

            return result >= 0;
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

        private List<PublicGroupMessageDto> ReturnDtoListFromReader(List<PublicGroupMessageDto> dtoList, SqlDataReader reader)
        {
            while (reader.Read())
            {
                PublicGroupMessageDto dto = new()
                {
                    PublicMessageId = (Guid)reader[0],
                    UserId          = (string)reader[1],
                    UserName        = (string)reader[2],
                    ChatGroupId     = (int)reader[3],
                    ChatGroupName   = (string)reader[4],
                    Text            = (string)reader[5],
                    MessageDateTime = (DateTime)reader[6],
                    ReplyMessageId  = reader[7].ToString().IsNullOrEmpty() ? null : (Guid)reader[7],
                    PictureLink     = reader[8].ToString().IsNullOrEmpty() ? null : reader[8].ToString()
                };
                dtoList.Add(dto);
            }
            return dtoList;
        }

        private PublicGroupMessageDto ReturnDtoFromReader(PublicGroupMessageDto dto, SqlDataReader reader)
        {
            while (reader.Read())
            {
                dto.PublicMessageId = (Guid)reader[0];
                dto.UserId          = (string)reader[1];
                dto.UserName        = (string)reader[2];
                dto.ChatGroupId     = (int)reader[3];
                dto.ChatGroupName   = (string)reader[4];
                dto.Text            = (string)reader[5];
                dto.MessageDateTime = (DateTime)reader[6];
                dto.ReplyMessageId  = reader[7].ToString().IsNullOrEmpty() ? null : (Guid)reader[7];
                dto.PictureLink     = reader[8].ToString().IsNullOrEmpty() ? null : reader[8].ToString();
            }
            return dto;
        }

        #endregion
    }
}
