﻿using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using System.Data.Entity;
using System.Data.SqlClient;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class PrivateMessagesDataAccess : IPrivateMessagesDataAccess
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PrivateMessagesDataAccess(ApplicationDbContext context, IConfiguration configuration)
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
            command.Parameters.Add("@userId", System.Data.SqlDbType.Int).Value = userId;
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

        public async Task<bool> Exists(Guid messageId)
        {
            return await _context.PrivateGroupMessages
                .AnyAsync(p => p.PrivateMessageId == messageId);
        }

        public async Task<bool> AddAsync(PrivateUserMessages newMessage)
        {
            _context.PrivateUserMessages.Add(newMessage);
            return await Save();
        }

        public async Task<bool> ModifyAsync(PrivateUserMessages modifyMessage)
        {
            _context.PrivateUserMessages.Update(modifyMessage);
            return await Save();
        }

        public async Task<bool> DeleteAsync(PrivateUserMessages deleteMessage)
        {
            _context.PrivateUserMessages.Remove(deleteMessage);
            return await Save();
        }

        public async Task<bool> DeleteMessagesByResponseMessageIdAsync(Guid responseMessageId)
        {
            int result = await _context.PrivateGroupMessages
                .Where(r => r.ReplyMessageId == responseMessageId)
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
