using ChatApplicationModels;
using Microsoft.Extensions.Configuration;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using static SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateMessagesTests.PrivateGroupMessagesDatabaseFixture;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateMessagesTests
{
    public class PrivateGroupMessagesDataAccess_UnitTests : IClassFixture<PrivateGroupMessagesDatabaseFixture>
    {
        public PrivateGroupMessagesDatabaseFixture Fixture { get; }
        private readonly PrivateGroupMessagesDataAccess _dataAccess;
        private readonly TestPrivateGroupMessagesDbContext _context;
        private readonly IConfiguration _configuration;

        public PrivateGroupMessagesDataAccess_UnitTests(PrivateGroupMessagesDatabaseFixture fixture)
        {
            Fixture = fixture;
            _context = Fixture.CreateContext();
            _configuration = new Mock<IConfiguration>().Object;
            _dataAccess = new PrivateGroupMessagesDataAccess(_context, _configuration);
        }

        [Fact]
        public async Task GetDtoListByGroupIdAsync_ReturnsMessages()
        {
            int groupId = 1;
            List<PrivateGroupMessageDto> dtoList = _context.PrivateMessagesDto
                .Where(p => p.ChatGroupId == groupId)
                .ToList();

            Mock<IPrivateGroupMessagesDataAccess> _mockPrivateMessagesDataAccess = new();
            _mockPrivateMessagesDataAccess.Setup(p => p.GetDtoListByGroupIdAsync(groupId, 0))
                .ReturnsAsync(dtoList);

            var mockedDataAccess = _mockPrivateMessagesDataAccess.Object;
            List<PrivateGroupMessageDto> result = await mockedDataAccess.GetDtoListByGroupIdAsync(groupId, 0);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Equal(dtoList, result);
            });
        }

        [Fact]
        public async Task GetDtoListByUserIdAsync_ReturnsMessages()
        {
            string userId = "e1b9cf9a-ff86-4607-8765-9e47a305062a";
            List<PrivateGroupMessageDto> dtoList = _context.PrivateMessagesDto
                .Where(p => p.UserId == userId)
                .ToList();

            Mock<IPrivateGroupMessagesDataAccess> _mockPrivateMessagesDataAccess = new();
            _mockPrivateMessagesDataAccess.Setup(p => p.GetDtoListByUserIdAsync(userId, 0))
                .ReturnsAsync(dtoList);

            var mockedDataAccess = _mockPrivateMessagesDataAccess.Object;
            List<PrivateGroupMessageDto> result = await mockedDataAccess.GetDtoListByUserIdAsync(userId, 0);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Equal(dtoList, result);
            });
        }

        [Fact]
        public async Task GetDtoByMessageIdAsync_ReturnsMessage()
        {
           Guid messageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            PrivateGroupMessageDto dto = _context.PrivateMessagesDto
                 .Single(p => p.PrivateMessageId == messageId);

            Mock<IPrivateGroupMessagesDataAccess> _mockPrivateMessagesDataAccess = new();
            _mockPrivateMessagesDataAccess.Setup(p => p.GetDtoByMessageIdAsync(messageId))
                .ReturnsAsync(dto);

            var mockedDataAccess = _mockPrivateMessagesDataAccess.Object;
            PrivateGroupMessageDto result = await mockedDataAccess.GetDtoByMessageIdAsync(messageId);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Equal(dto, result);
            });
        }

        [Fact]
        public async Task GetByMessageIdAsync_ReturnsMessage()
        {
            Guid messageId = _context.PrivateGroupMessages.First().PrivateMessageId;
            
            PrivateGroupMessages result = await _dataAccess.GetByMessageIdAsync(messageId);

            Assert.Equal(messageId, result.PrivateMessageId);
        }

        [Fact]
        public async Task Exists_ReturnsCorrectResult()
        {
            Guid goodMessageId = _context.PrivateGroupMessages.First().PrivateMessageId;
            Guid badMessageId = Guid.Parse("00000000-0000-0000-0000-000000000000");

            bool messageExistsTrue = await _dataAccess.MessageIdExists(goodMessageId);
            bool messageExistsFalse = await _dataAccess.MessageIdExists(badMessageId);

            Assert.Multiple(() =>
            {
                Assert.True(messageExistsTrue);
                Assert.False(messageExistsFalse);
            });
        }

        [Fact]
        public async Task AddAsync_IsSuccess()
        {
            PrivateGroupMessages newMessage = GetNewMessage();
            Guid newMessageId = newMessage.PrivateMessageId;

            _context.Database.BeginTransaction();
            bool addResult = await _dataAccess.AddAsync(newMessage);
            _context.ChangeTracker.Clear();

            var addedMessage = _context.PrivateGroupMessages
                .Single(p => p.PrivateMessageId == newMessageId);

            Assert.Multiple(() =>
            {
                Assert.True(addResult);
                Assert.Equal(addedMessage.UserId, newMessage.UserId);
            });
        }

        [Fact]
        public async Task ModifyAsync_IsSuccess()
        {
            PrivateGroupMessages messageToModify = _context.PrivateGroupMessages.First();
            string newMessage = "modified message";
            messageToModify.Text = newMessage;

            _context.Database.BeginTransaction();
            bool modifyResult = await _dataAccess.ModifyAsync(messageToModify);
            _context.ChangeTracker.Clear();

            var result = _context.PrivateGroupMessages
                .Single(p => p.PrivateMessageId == messageToModify.PrivateMessageId);

            Assert.Multiple(() =>
            {
                Assert.True(modifyResult);
                Assert.Equal(newMessage, result.Text);
            });
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            PrivateGroupMessages messageToDelete = _context.PrivateGroupMessages.First();

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.DeleteAsync(messageToDelete);
            _context.ChangeTracker.Clear();

            bool messageExists = await _dataAccess.MessageIdExists(messageToDelete.PrivateMessageId);

            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.False(messageExists);
            });
        }

        [Fact]
        public async Task DeleteMessagesByReplyMessageIdAsync_IsSuccess()
        {
            // 2nd message in fixture data has this as replymessageid
            Guid replyMessageId = _context.PrivateGroupMessages.First().PrivateMessageId;

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.DeleteMessagesByReplyMessageIdAsync(replyMessageId);
            _context.ChangeTracker.Clear();

            int count = _context.PrivateGroupMessages
                .Where(r => r.ReplyMessageId == replyMessageId)
                .Count();

            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.Equal(0, count);
            });
        }

        [Fact]
        public async Task DeleteAllMessagesInGroupAsync_IsSuccess()
        {
            int groupId = 1;

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.DeleteAllMessagesInGroupAsync(groupId);
            _context.ChangeTracker.Clear();

            int count = _context.PrivateGroupMessages
                .Where(c => c.ChatGroupId == groupId)
                .Count();

            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.Equal(0, count);
            });
        }

        #region PRIVATE METHODS

        private PrivateGroupMessages GetNewMessage()
        {
            return new()
            {
                PrivateMessageId = Guid.NewGuid(),
                ChatGroupId      = 1,
                UserId           = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                MessageDateTime  = DateTime.UtcNow,
                Text             = "Test"
            };
        }

        #endregion
    }
}
