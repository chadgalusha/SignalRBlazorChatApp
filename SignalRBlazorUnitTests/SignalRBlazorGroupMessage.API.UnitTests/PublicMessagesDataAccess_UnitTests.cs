using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class PublicMessagesDataAccess_UnitTests : IClassFixture<TestPublicMessagesDatabaseFixture>
    {
        public TestPublicMessagesDatabaseFixture Fixture { get; }
        private readonly PublicMessagesDataAccess _dataAccess;
        private readonly ApplicationDbContext _context;

        public PublicMessagesDataAccess_UnitTests(TestPublicMessagesDatabaseFixture fixture)
        {
            Fixture = fixture;
            _context = Fixture.CreateContext();
            ISerilogger serilogger = new Serilogger();
            _dataAccess = new PublicMessagesDataAccess(_context, serilogger);
        }

        [Fact]
        public async Task GetPublicMessagesByGroupId_ReturnsMessages()
        {
            List<PublicMessages> listMessages = await _dataAccess.GetMessagesByGroupIdAsync(2);

            string expectedPublicMessageId = "512fce5e-865a-4e4d-b6fd-2a57fb86149e";
            string actualPublicMessageId = listMessages
                                            .Where(c => c.UserId == "feac8ce0-5a21-4b89-9e23-beee9df517bb")
                                            .First()
                                            .PublicMessageId;

            Assert.Equal(2, listMessages.Count);
            Assert.Equal(expectedPublicMessageId, actualPublicMessageId);
        }

        [Fact]
        public async Task GetPublicMessagesByUserId_ReturnsMessages()
        {
            string userId = "e1b9cf9a-ff86-4607-8765-9e47a305062a";

            List<PublicMessages> listMessages = await _dataAccess.GetMessagesByUserIdAsync(userId);

            Assert.True(listMessages.Count > 0);
        }

        [Fact]
        public async Task AddPublicMessages_IsSuccess()
        {
            PublicMessages newMessage = NewPublicMessage();
            string expectedPublicMessageId = newMessage.PublicMessageId;

            _context.Database.BeginTransaction();
            await _dataAccess.AddMessageAsync(newMessage);
            _context.ChangeTracker.Clear();

            List<PublicMessages> listMessages = await _dataAccess.GetMessagesByGroupIdAsync(3);
            string resultpublicMessageId = listMessages.First().PublicMessageId;

            Assert.Single(listMessages);
            Assert.Equal(expectedPublicMessageId, resultpublicMessageId);
        }

        // check if 1st public message in Fixture class GetListPublicMessages() is present. 2nd check should be false.
        [Fact]
        public async Task PublicMessageExists_IsSuccess()
        {
            bool messageExistsTrue = await _dataAccess.PublicMessageExists("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            bool messageExistsFalse = await _dataAccess.PublicMessageExists("00000000-0000-0000-0000-000000000000");

            Assert.True(messageExistsTrue);
            Assert.False(messageExistsFalse);
        }

        [Fact]
        public async Task ModifyMessageAsync_IsSuccess()
        {
            string newMessage = "Modified message";
            PublicMessages publicMessageToModify = await _dataAccess.GetPublicMessageByIdAsync("e8ee70b6-678a-4b86-934e-da7f404a33a3");
            publicMessageToModify.Text = newMessage;

            _context.Database.BeginTransaction();
            await _dataAccess.ModifyMessageAsync(publicMessageToModify);
            _context.ChangeTracker.Clear();

            PublicMessages modifiedPublicMessage = await _dataAccess.GetPublicMessageByIdAsync("e8ee70b6-678a-4b86-934e-da7f404a33a3");

            Assert.Equal(newMessage, modifiedPublicMessage.Text);
        }

        [Fact]
        public async Task DeleteMessage_IsSuccess()
        {
            string publicMessageId = "e8ee70b6-678a-4b86-934e-da7f404a33a3";
            PublicMessages publicMessageToDelete = await _dataAccess.GetPublicMessageByIdAsync(publicMessageId);

            _context.Database.BeginTransaction();
            await _dataAccess.DeleteMessage(publicMessageToDelete);
            _context.ChangeTracker.Clear();

            bool messageExists = await _dataAccess.PublicMessageExists(publicMessageId);
            Assert.False(messageExists);
        }

        #region PRIVATE METHODS

        private PublicMessages NewPublicMessage()
        {
            return new()
            {
                PublicMessageId = new Guid().ToString(),
                UserId = "e1b9cf9a-ff86-4607-8765-9e47a305062a",
                ChatGroupId = 3,
                Text = "Sample message for ChatGroupId 3",
                MessageDateTime = new DateTime(2023, 6, 25)
            };
        }

        #endregion
    }
}
