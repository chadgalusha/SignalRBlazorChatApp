using ChatApplicationModels;
using Microsoft.Extensions.Configuration;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Models;
using static TestPublicMessagesDatabaseFixture;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class PublicMessagesDataAccess_UnitTests : IClassFixture<TestPublicMessagesDatabaseFixture>
    {
        public TestPublicMessagesDatabaseFixture Fixture { get; }
        private readonly PublicMessagesDataAccess _dataAccess;
        private readonly TestPublicMessagesDbContext _context;
        private readonly IConfiguration _configuration;

        public PublicMessagesDataAccess_UnitTests(TestPublicMessagesDatabaseFixture fixture)
        {
            Fixture = fixture;
            _context = Fixture.CreateContext();
            _configuration = new Mock<IConfiguration>().Object;
            _dataAccess = new PublicMessagesDataAccess(_context, _configuration);
        }

        [Fact]
        public async Task GetViewListByGroupId_ReturnsMessages()
        {
            int groupId = 2;
            List<PublicMessagesView> publicMessagesViewsList = _context.PublicMessagesView
                .Where(p => p.ChatGroupId == groupId)
                .ToList();

            Mock<IPublicMessagesDataAccess> _mockPublicMessageDataAccess = new();
            _mockPublicMessageDataAccess.Setup(x => x.GetViewListByGroupIdAsync(groupId, 0))
                .ReturnsAsync(publicMessagesViewsList);

            var mockedDataAccessObject = _mockPublicMessageDataAccess.Object;
            List<PublicMessagesView> result = await mockedDataAccessObject.GetViewListByGroupIdAsync(groupId, 0);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Equal(publicMessagesViewsList, result);
            });
        }

        [Fact]
        public async Task GetViewListByUserId_ReturnsMessages()
        {
            Guid userId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a");
            List<PublicMessagesView> listPublicMessageView = _context.PublicMessagesView
                .Where(p => p.UserId == userId)
                .ToList();

            Mock<IPublicMessagesDataAccess> _mockPublicMessageDataAccess = new();
            _mockPublicMessageDataAccess.Setup(x => x.GetViewListByUserIdAsync(userId, 0))
                .ReturnsAsync(listPublicMessageView);

            var mockedDataAccessObject = _mockPublicMessageDataAccess.Object;
            List<PublicMessagesView> result = await mockedDataAccessObject.GetViewListByUserIdAsync(userId, 0);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Equal(listPublicMessageView, result);
                Assert.Single(listPublicMessageView);
            });
        }

        // Add 1 new PublicMessages. Check table in db after adding.
        [Fact]
        public async Task AddPublicMessages_IsSuccess()
        {
            PublicMessages newMessage = NewPublicMessage();
            Guid expectedPublicMessageId = newMessage.PublicMessageId;

            _context.Database.BeginTransaction();
            await _dataAccess.AddAsync(newMessage);
            _context.ChangeTracker.Clear();

            List<PublicMessages> listMessages = _context.PublicMessages
                .Where(p => p.ChatGroupId == 3)
                .ToList();
            Guid resultpublicMessageId = listMessages.First().PublicMessageId;

            Assert.Multiple(() =>
            {
                Assert.Single(listMessages);
                Assert.Equal(expectedPublicMessageId, resultpublicMessageId);
            });
        }

        // check if 1st public message in Fixture class GetListPublicMessages() is present. 2nd check should be false.
        [Fact]
        public async Task PublicMessageExists_IsSuccess()
        {
            Guid goodId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            Guid badId = Guid.Parse("00000000-0000-0000-0000-000000000000");

            bool messageExistsTrue = await _dataAccess.Exists(goodId);
            bool messageExistsFalse = await _dataAccess.Exists(badId);

            Assert.Multiple(() =>
            {
                Assert.True(messageExistsTrue);
                Assert.False(messageExistsFalse);
            });
        }

        [Fact]
        public async Task ModifyMessageAsync_IsSuccess()
        {
            Guid messageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            string newMessage = "Modified message";
            PublicMessages messageToModify = _context.PublicMessages
                .Single(m => m.PublicMessageId == messageId);
            messageToModify.Text = newMessage;

            _context.Database.BeginTransaction();
            bool resultOfModify = await _dataAccess.ModifyAsync(messageToModify);
            _context.ChangeTracker.Clear();

            PublicMessages modifiedPublicMessage = _context.PublicMessages
                .Single(p => p.PublicMessageId == messageId);

            Assert.Multiple(() =>
            {
                Assert.True(resultOfModify);
                Assert.Equal(newMessage, modifiedPublicMessage.Text);
            });
        }

        [Fact]
        public async Task DeleteMessage_IsSuccess()
        {
            Guid publicMessageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            PublicMessages publicMessageToDelete = _context.PublicMessages
                .Single(p => p.PublicMessageId == publicMessageId);

            _context.Database.BeginTransaction();
            bool resultOfDelete = await _dataAccess.DeleteAsync(publicMessageToDelete);
            _context.ChangeTracker.Clear();

            bool messageExists = await _dataAccess.Exists(publicMessageId);
            Assert.Multiple(() =>
            {
                Assert.True(resultOfDelete);
                Assert.False(messageExists);
            });
        }

        [Fact]
        public async Task GetByMessageIdAsync_ReturnsCorrectResult()
        {
            Guid messageId = Guid.Parse("3eea1c79-61fb-41e0-852b-ab790835c827");

            PublicMessages result = await _dataAccess.GetByMessageIdAsync(messageId);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteMessagesByResponseMessageId_IsSucess()
        {
            Guid messageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.DeleteMessagesByResponseMessageIdAsync(messageId);
            _context.ChangeTracker.Clear();

            int count = _context.PublicMessages
                .Where(r => r.ReplyMessageId == messageId)
                .Count();

            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.Equal(0, count);
            });
        }

        #region PRIVATE METHODS

        private PublicMessages NewPublicMessage()
        {
            return new()
            {
                PublicMessageId = Guid.NewGuid(),
                UserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                ChatGroupId = 3,
                Text = "Sample message for ChatGroupId 3",
                MessageDateTime = new DateTime(2023, 6, 25)
            };
        }

        #endregion
    }
}
