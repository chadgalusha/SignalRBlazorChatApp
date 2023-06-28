using ChatApplicationModels;
using Moq;
using SignalRBlazorGroupsMessages.API.Data;
using SignalRBlazorGroupsMessages.API.DataAccess;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class ChatGroupsDataAccess_UnitTests : IClassFixture<TestChatGroupsDatabaseFixture>
    {
        public TestChatGroupsDatabaseFixture Fixture { get; }
        private readonly ChatGroupsDataAccess _dataAccess;
        private readonly ApplicationDbContext _context;

        public ChatGroupsDataAccess_UnitTests(TestChatGroupsDatabaseFixture fixture)
        {
            Fixture = fixture;
            _context = Fixture.CreateContext();
            _dataAccess = new ChatGroupsDataAccess(_context);
        }

        [Fact]
        public async Task GetPublicChatGroupsAsync_ReturnsChatGroups()
        {
            int expectedPublicChatGroups = 2;
            string expectedChatGroupName_Id2 = "TestPublicGroup2";

            List<ChatGroups> listPublicChatGroups = await _dataAccess.GetPublicChatGroupsAsync();
            string resultChatGroupName = listPublicChatGroups
                .Single(n => n.ChatGroupId == 2)
                .ChatGroupName;

            Assert.Multiple(() =>
            {
                Assert.Equal(expectedPublicChatGroups, listPublicChatGroups.Count);
                Assert.True(resultChatGroupName != null);
                Assert.Equal(expectedChatGroupName_Id2, resultChatGroupName);
            });
        }

        [Fact]
        public async Task GetChatGroupById_ReturnsCorrectChatGroup()
        {
            string expectedChatGroupName = "TestPublicGroup1";

            ChatGroups resultChatGroup = await _dataAccess.GetChatGroupByIdAsync(1);

            Assert.Equal(expectedChatGroupName, resultChatGroup.ChatGroupName);
        }

        [Fact]
        public async Task ChatGroupExists_ReturnsCorrectResult()
        {
            bool shouldBeTrue = await _dataAccess.ChatGroupexists(1);
            bool shouldBeFalse = await _dataAccess.ChatGroupexists(999);

            Assert.Multiple(() =>
            {
                Assert.True(shouldBeTrue);
                Assert.False(shouldBeFalse);
            });
        }

        [Fact]
        public async Task AddChatGroupAsync_IsSuccess()
        {
            string expectedNewChatGroupName = "NewPublicChatGroup";
            ChatGroups newChatGroup = GetNewPublicChatGroup();

            _context.Database.BeginTransaction();
            await _dataAccess.AddChatGroupAsync(newChatGroup);
            _context.ChangeTracker.Clear();

            List<ChatGroups> listChatGroups = await _dataAccess.GetPublicChatGroupsAsync();
            string resultNewChatGroupName = listChatGroups.Last().ChatGroupName;

            Assert.Multiple(() =>
            {
                Assert.Equal(3, listChatGroups.Count);
                Assert.Equal(expectedNewChatGroupName, resultNewChatGroupName);
            });
        }


        [Fact]
        public async Task ModifyChatGroup_IsSuccess()
        {
            string expectedModifiedGroupName = "ModifiedChatGroupName";
            ChatGroups chatGroupToModify = await _dataAccess.GetChatGroupByIdAsync(1);
            chatGroupToModify.ChatGroupName = expectedModifiedGroupName;

            _context.Database.BeginTransaction();
            await _dataAccess.ModifyChatGroup(chatGroupToModify);
            _context.ChangeTracker.Clear();

            ChatGroups modifiedChatGroup = await _dataAccess.GetChatGroupByIdAsync(1);

            Assert.Equal(expectedModifiedGroupName, modifiedChatGroup.ChatGroupName);
        }

        [Fact]
        public async Task DeleteChatGroupAsync_IsSuccess()
        {
            ChatGroups chatGroupToDelete = await _dataAccess.GetChatGroupByIdAsync(1);
            int chatGroupToDeleteId = chatGroupToDelete.ChatGroupId;

            _context.Database.BeginTransaction();
            await _dataAccess.DeleteChatGroupAsync(chatGroupToDelete);
            _context.ChangeTracker.Clear();

            bool chatGroupExists = await _dataAccess.ChatGroupexists(chatGroupToDeleteId);
            Assert.False(chatGroupExists);
        }

        [Fact]
        public async Task AddUserToPrivateChatGroup_AddsUser()
        {
            int groupToJoinId = 4;
            string userIdToJoin = "e08b0077-3c15-477e-84bb-bf9d41196455";

            _context.Database.BeginTransaction();
            await _dataAccess.AddUserToPrivateChatGroup(groupToJoinId, userIdToJoin);
            _context.ChangeTracker.Clear();

            List<PrivateGroupMembers> listPrivateGroupMembers = _context.PrivateGroupsMembers
                .Where(c => c.UserId == userIdToJoin)
                .ToList();
            bool isUserInGroup = _context.PrivateGroupsMembers
                .Where(p => p.PrivateChatGroupId == groupToJoinId
                    && p.UserId == userIdToJoin)
                .Any();
            
            Assert.Multiple(() =>
            {
                Assert.Equal(2, listPrivateGroupMembers.Count);
                Assert.True(isUserInGroup);
            });
        }

        [Fact]
        public async Task RemoveUserFromPrivateChatGroup_RemovesUser()
        {
            string userId = "e08b0077-3c15-477e-84bb-bf9d41196455";
            int recordIdToDelete = _context.PrivateGroupsMembers
                .Single(p => p.UserId == userId)
                .PrivateGroupMemberId;

            _context.Database.BeginTransaction();
            await _dataAccess.RemoveUserFromPrivateChatGroup(recordIdToDelete, userId);
            _context.ChangeTracker.Clear();

            bool recordExists = _context.PrivateGroupsMembers
                .Where(p => p.PrivateGroupMemberId == recordIdToDelete)
                .Any();

            Assert.False(recordExists);
        }

        [Fact]
        public void GetPrivateChatGroupsByUserId_ReturnsCorrectList()
        {
            string userId = "e08b0077-3c15-477e-84bb-bf9d41196455";
            List<ChatGroups> listPrivateChatGroups = GetListPrivateChatGroups(userId);

            Mock<IChatGroupsDataAccess> _mockDataAccess = new();
            _mockDataAccess.Setup(x => x.GetPrivateChatGroupsByUserId(userId))
                .Returns(listPrivateChatGroups);

            var mockedDataAccessObject = _mockDataAccess.Object;
            var result = mockedDataAccessObject.GetPrivateChatGroupsByUserId(userId);

            Assert.Equal(listPrivateChatGroups, result);
        }

        #region PRIVATE METHODS

        private ChatGroups GetNewPublicChatGroup()
        {
            return new()
            {
                ChatGroupName = "NewPublicChatGroup",
                GroupCreated = DateTime.Now,
                GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071",
                PrivateGroup = false
            };
        }

        // Mock of stored procedure sp_getPrivateChatGroupsForUser @UserId
        private List<ChatGroups> GetListPrivateChatGroups(string userId)
        {
            List<PrivateGroupMembers> listPrivateGroupMembers = _context.PrivateGroupsMembers
                .Where(p => p.UserId == userId)
                .ToList();

            List<ChatGroups> listPrivateChatGroups = new();
            foreach (var listItem in listPrivateGroupMembers)
            {
                ChatGroups chatGroup = _context.ChatGroups
                    .Single(c => c.ChatGroupId == listItem.PrivateChatGroupId);

                if (chatGroup != null)
                {
                    listPrivateChatGroups.Add(chatGroup);
                }
            }

            return listPrivateChatGroups;
        }
        #endregion
    }
}
