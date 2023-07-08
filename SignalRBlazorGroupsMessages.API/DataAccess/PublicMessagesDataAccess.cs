using ChatApplicationModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.Models;

namespace SignalRBlazorGroupsMessages.API.DataAccess
{
    public class PublicMessagesDataAccess : IPublicMessagesDataAccess
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PublicMessagesDataAccess(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new Exception(nameof(context));
            _configuration = configuration ?? throw new Exception(nameof(configuration));
        }

        public async Task<List<PublicMessagesView>> GetViewListByGroupIdAsync(int groupId, int numberItemsToSkip)
        {
            List<PublicMessagesView> listView = new();
            string connectionString = _configuration.GetConnectionString("ChatApplicationDb");

            using SqlConnection connection = new(connectionString);
            SqlCommand command = new("sp_getPublicMessages_byGroupId", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.Add("@groupId", System.Data.SqlDbType.Int).Value = groupId;
            command.Parameters.Add("@numberMessagesToSkip", System.Data.SqlDbType.Int).Value = numberItemsToSkip;

            await connection.OpenAsync();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                PublicMessagesView view = new()
                {
                    PublicMessageId = Guid.Parse((string)reader[0]),
                    UserId          = Guid.Parse((string)reader[1]),
                    UserName        = (string)reader[2],
                    ChatGroupId     = (int)reader[3],
                    ChatGroupName   = (string)reader[4],
                    Text            = (string)reader[5],
                    MessageDateTime = (DateTime)reader[6],
                    ReplyMessageId  = reader[7].ToString().IsNullOrEmpty() ? null : Guid.Parse((string)reader[7]),
                    PictureLink     = reader[8].ToString().IsNullOrEmpty() ? null : reader[8].ToString()
                };
                listView.Add(view);
            }

            await connection.CloseAsync();
            return listView;
        }

        public async Task<List<PublicMessagesView>> GetViewListByUserIdAsync(Guid userId, int numberItemsToSkip)
        {
            return await _context.Database
                .SqlQuery<PublicMessagesView>($"EXECUTE sp_getPublicMessage_byMessageId @userId={userId}, @numberMessagesToSkip={numberItemsToSkip}")
                .ToListAsync();
        }

        public async Task<PublicMessagesView> GetViewByMessageIdAsync(Guid messageId)
        {
            return await _context.Database
                .SqlQuery<PublicMessagesView>($"EXECUTE sp_getPublicMessage_byMessageId @messageId={messageId}")
                .SingleAsync();
        }

        public async Task<PublicMessages> GetByMessageIdAsync(Guid messageId)
        {
            return await _context.PublicMessages.SingleAsync(p => p.PublicMessageId == messageId);
        }

        public async Task<bool> Exists(Guid messageId)
        {
            return await _context.PublicMessages
                .AnyAsync(p => p.PublicMessageId == messageId);
        }

        public async Task<bool> AddAsync(PublicMessages message)
        {
            await _context.PublicMessages.AddAsync(message);
            return await Save();
        }

        public async Task<bool> ModifyAsync(PublicMessages message)
        {
            return await Save();
        }

        public async Task<bool> DeleteAsync(PublicMessages message)
        {
            _context.PublicMessages.Remove(message);
            return await Save();
        }

        public async Task<bool> DeleteMessagesByResponseMessageIdAsync(Guid responseMessageId)
        {
            int result = await _context.PublicMessages
                .Where(r => r.ReplyMessageId == responseMessageId)
                .ExecuteDeleteAsync();

            return result >= 0;
        }

        #region PRIVATE METHODS

        private async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        #endregion
    }
}
