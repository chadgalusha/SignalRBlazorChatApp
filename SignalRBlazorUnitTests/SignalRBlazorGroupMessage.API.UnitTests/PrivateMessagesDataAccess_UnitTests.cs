using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class PrivateMessagesDataAccess_UnitTests : IClassFixture<TestPrivateMessagesDatabaseFixture>
    {
        public TestPrivateMessagesDatabaseFixture Fixture { get; }
        private readonly PrivateMessagesDataAccess _dataAccess;
        private readonly ApplicationDbContext _context;

        public PrivateMessagesDataAccess_UnitTests(TestPrivateMessagesDatabaseFixture fixture)
        {
            Fixture = fixture;
            _context = Fixture.CreateContext();
            _dataAccess = new PrivateMessagesDataAccess(_context);
        }

        [Fact]
        public async Task GetAllPrivateMessagesForUserAsync_ReturnsExpectedMessages()
        {
            string validUserId = "e08b0077-3c15-477e-84bb-bf9d41196455";
            string invalidUserId = "00000000-0000-0000-0000-000000000000";

            List<PrivateMessages> listValidUserPrivateMessages = await _dataAccess.GetAllPrivateMessagesForUserAsync(validUserId);
            List<PrivateMessages> listInvalidUserPrivateMessages = await _dataAccess.GetAllPrivateMessagesForUserAsync(invalidUserId);

            Assert.Multiple(() =>
            {
                Assert.Equal(3, listValidUserPrivateMessages.Count);
                Assert.Empty(listInvalidUserPrivateMessages);
            });
        }

        [Fact]
        public async Task GetPrivateMessagesFromUserAsync_ReturnsCorrectResult()
        {
            string toUserId = "e08b0077-3c15-477e-84bb-bf9d41196455";
            string fromUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071";
            string invalidUserId = "00000000-0000-0000-0000-000000000000";

            List<PrivateMessages> listValidPrivateMessages = await _dataAccess.GetPrivateMessagesFromUserAsync(toUserId, fromUserId);
            List<PrivateMessages> listInvalidPrivateMessages = await _dataAccess.GetPrivateMessagesFromUserAsync(toUserId, invalidUserId);

            Assert.Multiple(() =>
            {
                Assert.Equal(2, listValidPrivateMessages.Count);
                Assert.Empty(listInvalidPrivateMessages);
            });
        }

        [Fact]
        public void GetPrivateMessage_ReturnsCorrectResult()
        {
            int validMessageId = 1;

            PrivateMessages validPrivateMessage = _dataAccess.GetPrivateMessage(validMessageId);

            Assert.Equal("Test Message 1", validPrivateMessage.MessageText);
        }

        [Fact]
        public void GetPrivateMessage_InvalidIdThrowsError()
        {
            int invalidMessageId = 999;

            Assert.Throws<GroupsMessagesExceptions>(() =>
            {
                PrivateMessages message = _dataAccess.GetPrivateMessage(invalidMessageId);
            });
        }

        [Fact]
        public async Task AddPrivateMessageAsync_IsSuccess()
        {
            PrivateMessages newPrivateMessage = GetNewPrivateMessage();
            int expectedCountAfterAdd = 4;
            string userId = "e08b0077-3c15-477e-84bb-bf9d41196455";

            _context.Database.BeginTransaction();
            await _dataAccess.AddPrivateMessageAsync(newPrivateMessage);
            _context.ChangeTracker.Clear();

            List<PrivateMessages> resultListExpctedMessages = await _dataAccess.GetAllPrivateMessagesForUserAsync(userId);
            Assert.Equal(expectedCountAfterAdd, resultListExpctedMessages.Count);
        }

        [Fact]
        public async Task ModifyPrivateMessageAsync_IsSuccess()
        {
            PrivateMessages messageToModify = _dataAccess.GetPrivateMessage(1);
            string newText = "Modified Text";   
            messageToModify.MessageText = newText;

            _context.Database.BeginTransaction();
            await _dataAccess.ModifyPrivateMessageAsync(messageToModify);
            _context.ChangeTracker.Clear();

            PrivateMessages resultPrivateMessage = _dataAccess.GetPrivateMessage(1);
            Assert.Equal(newText, resultPrivateMessage.MessageText);
        }

        [Fact]
        public async Task DeletePrivateMessage_IsSuccess()
        {
            int messageToDeleteId = 1;
            PrivateMessages messageToDelete = _dataAccess.GetPrivateMessage(messageToDeleteId);

            _context.Database.BeginTransaction();
            await _dataAccess.DeletePrivateMessageAsync(messageToDelete);
            _context.ChangeTracker.Clear();

            bool result = _context.PrivateMessages.Where(p => p.PrivateMessageId == messageToDeleteId).Any();
            Assert.False(result);
        }

        #region PRIVATE METHODS

        private PrivateMessages GetNewPrivateMessage()
        {
            return new()
            {
                FromUserId = "2973b407-752e-4c6a-917c-f5e43ef98597",
                ToUserId = "e08b0077-3c15-477e-84bb-bf9d41196455",
                MessageText = "Test Message 5",
                MessageSeen = false,
                PrivateMessageDateTime = DateTime.Now
            };
        }

        #endregion
    }
}
