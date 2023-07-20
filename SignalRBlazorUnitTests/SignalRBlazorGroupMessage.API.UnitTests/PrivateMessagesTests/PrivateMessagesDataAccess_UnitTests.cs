﻿using ChatApplicationModels;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateMessages
{
    public class PrivateMessagesDataAccess_UnitTests : IClassFixture<PrivateMessagesDatabaseFixture>
    {
        public PrivateMessagesDatabaseFixture Fixture { get; }
        private readonly PrivateMessagesDataAccess _dataAccess;
        private readonly ApplicationDbContext _context;

        public PrivateMessagesDataAccess_UnitTests(PrivateMessagesDatabaseFixture fixture)
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

            List<PrivateUserMessages> listValidUserPrivateMessages = await _dataAccess.GetAllPrivateMessagesForUserAsync(validUserId);
            List<PrivateUserMessages> listInvalidUserPrivateMessages = await _dataAccess.GetAllPrivateMessagesForUserAsync(invalidUserId);

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

            List<PrivateUserMessages> listValidPrivateMessages = await _dataAccess.GetPrivateMessagesFromUserAsync(toUserId, fromUserId);
            List<PrivateUserMessages> listInvalidPrivateMessages = await _dataAccess.GetPrivateMessagesFromUserAsync(toUserId, invalidUserId);

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

            PrivateUserMessages validPrivateMessage = _dataAccess.GetPrivateMessage(validMessageId);

            Assert.Equal("Test Message 1", validPrivateMessage.MessageText);
        }

        [Fact]
        public void GetPrivateMessage_InvalidIdThrowsError()
        {
            int invalidMessageId = 999;

            Assert.Throws<GroupsMessagesExceptions>(() =>
            {
                PrivateUserMessages message = _dataAccess.GetPrivateMessage(invalidMessageId);
            });
        }

        [Fact]
        public async Task AddPrivateMessageAsync_IsSuccess()
        {
            PrivateUserMessages newPrivateMessage = GetNewPrivateMessage();
            int expectedCountAfterAdd = 4;
            string userId = "e08b0077-3c15-477e-84bb-bf9d41196455";

            _context.Database.BeginTransaction();
            await _dataAccess.AddAsync(newPrivateMessage);
            _context.ChangeTracker.Clear();

            List<PrivateUserMessages> resultListExpctedMessages = await _dataAccess.GetAllPrivateMessagesForUserAsync(userId);
            Assert.Equal(expectedCountAfterAdd, resultListExpctedMessages.Count);
        }

        [Fact]
        public async Task ModifyPrivateMessageAsync_IsSuccess()
        {
            PrivateUserMessages messageToModify = _dataAccess.GetPrivateMessage(1);
            string newText = "Modified Text";
            messageToModify.MessageText = newText;

            _context.Database.BeginTransaction();
            await _dataAccess.ModifyAsync(messageToModify);
            _context.ChangeTracker.Clear();

            PrivateUserMessages resultPrivateMessage = _dataAccess.GetPrivateMessage(1);
            Assert.Equal(newText, resultPrivateMessage.MessageText);
        }

        [Fact]
        public async Task DeletePrivateMessage_IsSuccess()
        {
            int messageToDeleteId = 1;
            PrivateUserMessages messageToDelete = _dataAccess.GetPrivateMessage(messageToDeleteId);

            _context.Database.BeginTransaction();
            await _dataAccess.DeleteAsync(messageToDelete);
            _context.ChangeTracker.Clear();

            bool result = _context.PrivateUserMessages.Where(p => p.PrivateMessageId == messageToDeleteId).Any();
            Assert.False(result);
        }

        #region PRIVATE METHODS

        private PrivateUserMessages GetNewPrivateMessage()
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
