using ChatApplicationModels;
using Microsoft.Extensions.Configuration;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using static SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PublicMessages.PublicMessagesDatabaseFixture;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PublicMessages
{
    public class PublicMessagesDataAccess_UnitTests : IClassFixture<PublicMessagesDatabaseFixture>
    {
        public PublicMessagesDatabaseFixture Fixture { get; }
        private readonly PublicMessagesDataAccess _dataAccess;
        private readonly TestPublicMessagesDbContext _context;
        private readonly IConfiguration _configuration;

        public PublicMessagesDataAccess_UnitTests(PublicMessagesDatabaseFixture fixture)
        {
            Fixture = fixture;
            _context = Fixture.CreateContext();
            _configuration = new Mock<IConfiguration>().Object;
            _dataAccess = new PublicMessagesDataAccess(_context, _configuration);
        }

        [Fact]
        public async Task GetDtoListByGroupId_ReturnsMessages()
        {
            int groupId = 2;
            List<PublicGroupMessageDto> publicMessagesDtoList = _context.PublicMessagesDto
                .Where(p => p.ChatGroupId == groupId)
                .ToList();

            Mock<IPublicMessagesDataAccess> _mockPublicMessageDataAccess = new();
            _mockPublicMessageDataAccess.Setup(x => x.GetDtoListByGroupIdAsync(groupId, 0))
                .ReturnsAsync(publicMessagesDtoList);

            var mockedDataAccessObject = _mockPublicMessageDataAccess.Object;
            List<PublicGroupMessageDto> result = await mockedDataAccessObject.GetDtoListByGroupIdAsync(groupId, 0);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Equal(publicMessagesDtoList, result);
            });
        }

        [Fact]
        public async Task GetDtoListByUserId_ReturnsMessages()
        {
            string userId = "e1b9cf9a-ff86-4607-8765-9e47a305062a";
            List<PublicGroupMessageDto> listPublicMessageDto = _context.PublicMessagesDto
                .Where(p => p.UserId == userId)
                .ToList();

            Mock<IPublicMessagesDataAccess> _mockPublicMessageDataAccess = new();
            _mockPublicMessageDataAccess.Setup(x => x.GetDtoListByUserIdAsync(userId, 0))
                .ReturnsAsync(listPublicMessageDto);

            var mockedDataAccessObject = _mockPublicMessageDataAccess.Object;
            List<PublicGroupMessageDto> result = await mockedDataAccessObject.GetDtoListByUserIdAsync(userId, 0);

            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Equal(listPublicMessageDto, result);
                Assert.Single(listPublicMessageDto);
            });
        }

        // Add 1 new PublicMessages. Check table in db after adding.
        [Fact]
        public async Task AddPublicMessages_IsSuccess()
        {
            PublicGroupMessages newMessage = NewPublicMessage();
            Guid expectedPublicMessageId = newMessage.PublicMessageId;

            _context.Database.BeginTransaction();
            await _dataAccess.AddAsync(newMessage);
            _context.ChangeTracker.Clear();

            List<PublicGroupMessages> listMessages = _context.PublicGroupMessages
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
        public async Task ModifyAsync_IsSuccess()
        {
            Guid messageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            string newMessage = "Modified message";
            PublicGroupMessages messageToModify = _context.PublicGroupMessages
                .Single(m => m.PublicMessageId == messageId);
            messageToModify.Text = newMessage;

            _context.Database.BeginTransaction();
            bool resultOfModify = await _dataAccess.ModifyAsync(messageToModify);
            _context.ChangeTracker.Clear();

            PublicGroupMessages modifiedPublicMessage = _context.PublicGroupMessages
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
            PublicGroupMessages publicMessageToDelete = _context.PublicGroupMessages
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

            PublicGroupMessages result = await _dataAccess.GetByMessageIdAsync(messageId);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteMessagesByResponseMessageId_IsSucess()
        {
            Guid messageId = Guid.Parse("e8ee70b6-678a-4b86-934e-da7f404a33a3");

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.DeleteMessagesByResponseMessageIdAsync(messageId);
            _context.ChangeTracker.Clear();

            int count = _context.PublicGroupMessages
                .Where(r => r.ReplyMessageId == messageId)
                .Count();

            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.Equal(0, count);
            });
        }

        [Fact]
        public async Task DeleteAllMessagesInGroup_IsSuccess()
        {
            int chatGroupToDeleteId = 1;
            int countShouldBeGreaterThanZero = _context.PublicGroupMessages
                .Where(c => c.ChatGroupId == chatGroupToDeleteId)
                .Count();

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.DeleteAllMessagesInGroupAsync(chatGroupToDeleteId);
            _context.ChangeTracker.Clear();

            int countShouldBeZero = _context.PublicGroupMessages
                .Where(c => c.ChatGroupId == chatGroupToDeleteId)
                .Count();

            Assert.Multiple(() =>
            {
                Assert.True(countShouldBeGreaterThanZero > 0);
                Assert.True(countShouldBeZero == 0);
                Assert.True(result);
            });

        }

        #region PRIVATE METHODS

        private PublicGroupMessages NewPublicMessage()
        {
            return new()
            {
                PublicMessageId = Guid.NewGuid(),
                UserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                ChatGroupId = 3,
                Text = "Sample message for ChatGroupId 3",
                MessageDateTime = new DateTime(2023, 6, 25)
            };
        }

        #endregion
    }
}
