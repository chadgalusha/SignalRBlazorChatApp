using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using System.Data.SqlClient;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class PrivateGroupMessagesDataAccess : IPrivateGroupMessagesDataAccess
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PrivateGroupMessagesDataAccess(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new Exception(nameof(context));
            _configuration = configuration ?? throw new Exception(nameof(configuration));
        }

        public async Task<List<PrivateGroupMessageDto>> GetDtoListByGroupIdAsync(int groupId, int numberMessagesToSkip)
        {
            List<PrivateGroupMessageDto> dtoList = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getPrivateGroupMessages_byGroupId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add("@groupId", System.Data.SqlDbType.Int).Value = groupId;
            command.Parameters.Add("@numberMessagesToSkip", System.Data.SqlDbType.Int).Value = numberMessagesToSkip;

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            dtoList = ReturnDtoListFromReader(dtoList, reader);
            await connection.CloseAsync();

            return dtoList;
        }

        public async Task<List<PrivateGroupMessageDto>> GetDtoListByUserIdAsync(string userId, int numberMessagesToSkip)
        {
            List<PrivateGroupMessageDto> dtoList = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getPrivateGroupMessages_byUserId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add("@userId", System.Data.SqlDbType.NVarChar).Value = userId;
            command.Parameters.Add("@numberMessagesToSkip", System.Data.SqlDbType.Int).Value = numberMessagesToSkip;

            await connection.OpenAsync();
            SqlDataReader reader = await command.ExecuteReaderAsync();
            dtoList = ReturnDtoListFromReader(dtoList, reader);
            await connection.CloseAsync();

            return dtoList;
        }

        public async Task<PrivateGroupMessageDto> GetDtoByMessageIdAsync(Guid messageId)
        {
            PrivateGroupMessageDto dto = new();

            using SqlConnection connection = new(GetConnectionString());
            SqlCommand command = new("sp_getPrivateGroupMessage_byMessageId", connection)
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

        public async Task<PrivateGroupMessages> GetByMessageIdAsync(Guid messageId)
        {
            return await _context.PrivateGroupMessages
                .SingleAsync(p => p.PrivateMessageId == messageId);
        }

        public async Task<bool> GroupIdExists(int groupId)
        {
            return await _context.PrivateGroupMessages
                .AnyAsync(g => g.ChatGroupId == groupId);
        }

        public async Task<bool> MessageIdExists(Guid messageId)
        {
            return await _context.PrivateGroupMessages
                .AnyAsync(p => p.PrivateMessageId == messageId);
        }

        public async Task<bool> AddAsync(PrivateGroupMessages newMessage)
        {
            _context.PrivateGroupMessages.Add(newMessage);
            return await Save();
        }

        public async Task<bool> ModifyAsync(PrivateGroupMessages modifyMessage)
        {
            _context.ChangeTracker.Clear();
            _context.PrivateGroupMessages.Update(modifyMessage);
            return await Save();
        }

        public async Task<bool> DeleteAsync(PrivateGroupMessages deleteMessage)
        {
            _context.PrivateGroupMessages.Remove(deleteMessage);
            return await Save();
        }

        public async Task<bool> DeleteMessagesByReplyMessageIdAsync(Guid replyMessageId)
        {
            int result = await _context.PrivateGroupMessages
                .Where(r => r.ReplyMessageId == replyMessageId)
                .ExecuteDeleteAsync();

            return result >= 0;
        }

        public async Task<bool> DeleteAllMessagesInGroupAsync(int chatgroupId)
        {
            int result = await _context.PrivateGroupMessages
                .Where(c => c.ChatGroupId == chatgroupId)
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

        private List<PrivateGroupMessageDto> ReturnDtoListFromReader(List<PrivateGroupMessageDto> dtoList, SqlDataReader reader)
        {
            while (reader.Read())
            {
                PrivateGroupMessageDto dto = new()
                {
                    PrivateMessageId = (Guid)reader[0],
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

        private PrivateGroupMessageDto ReturnDtoFromReader(PrivateGroupMessageDto dto, SqlDataReader reader)
        {
            while (reader.Read())
            {
                dto.PrivateMessageId = (Guid)reader[0];
                dto.UserId           = (string)reader[1];
                dto.UserName         = (string)reader[2];
                dto.ChatGroupId      = (int)reader[3];
                dto.ChatGroupName    = (string)reader[4];
                dto.Text             = (string)reader[5];
                dto.MessageDateTime  = (DateTime)reader[6];
                dto.ReplyMessageId   = reader[7].ToString().IsNullOrEmpty() ? null : (Guid)reader[7];
                dto.PictureLink      = reader[8].ToString().IsNullOrEmpty() ? null : reader[8].ToString();
            }
            return dto;
        }

        #endregion
    }
}
