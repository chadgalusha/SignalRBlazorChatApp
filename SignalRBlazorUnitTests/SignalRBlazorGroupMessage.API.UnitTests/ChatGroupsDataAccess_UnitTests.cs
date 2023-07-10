using ChatApplicationModels;
using Microsoft.Extensions.Configuration;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Models;
using static SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.TestChatGroupsDatabaseFixture;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class ChatGroupsDataAccess_UnitTests : IClassFixture<TestChatGroupsDatabaseFixture>
    {
        public TestChatGroupsDatabaseFixture Fixture { get; }
        private readonly ChatGroupsDataAccess _dataAccess;
        private readonly TestChatGroupsDbContext _context;
        private readonly IConfiguration _configuration;

        public ChatGroupsDataAccess_UnitTests(TestChatGroupsDatabaseFixture fixture)
        {
            Fixture = fixture;
            _context = Fixture.CreateContext();
            _configuration = new Mock<IConfiguration>().Object;
            _dataAccess = new ChatGroupsDataAccess(_context, _configuration);
        }

        [Fact]
        public async Task GetViewListPublicChatGroupsAsync_ReturnsChatGroups()
        {
            int expectedPublicChatGroupsCount = _context.ChatGroups
                .Where(c => c.PrivateGroup == false)
                .ToList()
                .Count;
            List<ChatGroupsView> viewList = _context.ChatGroupsViews
                .Where(c => c.PrivateGroup == false)
                .ToList();

            Mock<IChatGroupsDataAccess> _mockChatGroupsDataAccess = new();
            _mockChatGroupsDataAccess.Setup(c => c.GetViewListPublicChatGroupsAsync())
                .ReturnsAsync(viewList);

            var mockedDataAccessObject = _mockChatGroupsDataAccess.Object;
            List<ChatGroupsView> result = await mockedDataAccessObject.GetViewListPublicChatGroupsAsync();

            Assert.Multiple(() =>
            {
                Assert.Equal(expectedPublicChatGroupsCount, result.Count);
                Assert.NotNull(result);
            });
        }

        //[Fact]
        //public async Task GetChatGroupById_ReturnsCorrectChatGroup()
        //{
        //    string expectedChatGroupName = "TestPublicGroup1";

        //    ChatGroups resultChatGroup = await _dataAccess.GetChatGroupByIdAsync(1);

        //    Assert.Equal(expectedChatGroupName, resultChatGroup.ChatGroupName);
        //}

        [Fact]
        public void ChatGroupExists_ReturnsCorrectResult()
        {
            bool shouldBeTrue = _dataAccess.ChatGroupexists(1);
            bool shouldBeFalse = _dataAccess.ChatGroupexists(999);

            Assert.Multiple(() =>
            {
                Assert.True(shouldBeTrue);
                Assert.False(shouldBeFalse);
            });
        }

        //[Fact]
        //public async Task AddChatGroupAsync_IsSuccess()
        //{
        //    string expectedNewChatGroupName = "NewPublicChatGroup";
        //    ChatGroups newChatGroup = GetNewPublicChatGroup();

        //    _context.Database.BeginTransaction();
        //    bool resultOfAdd = await _dataAccess.AddChatGroupAsync(newChatGroup);
        //    _context.ChangeTracker.Clear();

        //    List<ChatGroups> listChatGroups = await _dataAccess.GetViewListPublicChatGroupsAsync();
        //    string resultNewChatGroupName = listChatGroups.Last().ChatGroupName;

        //    Assert.Multiple(() =>
        //    {
        //        Assert.True(resultOfAdd);
        //        Assert.Equal(3, listChatGroups.Count);
        //        Assert.Equal(expectedNewChatGroupName, resultNewChatGroupName);
        //    });
        //}


        //[Fact]
        //public async Task ModifyChatGroup_IsSuccess()
        //{
        //    string expectedModifiedGroupName = "ModifiedChatGroupName";
        //    ChatGroups chatGroupToModify = await _dataAccess.GetChatGroupByIdAsync(1);
        //    chatGroupToModify.ChatGroupName = expectedModifiedGroupName;

        //    _context.Database.BeginTransaction();
        //    bool resultOfModify = await _dataAccess.ModifyChatGroupAsync(chatGroupToModify);
        //    _context.ChangeTracker.Clear();

        //    ChatGroups modifiedChatGroup = await _dataAccess.GetChatGroupByIdAsync(1);

        //    Assert.Multiple(() =>
        //    {
        //        Assert.True(resultOfModify);
        //        Assert.Equal(expectedModifiedGroupName, modifiedChatGroup.ChatGroupName);
        //    });
        //}

        //[Fact]
        //public async Task DeleteChatGroupAsync_IsSuccess()
        //{
        //    ChatGroups chatGroupToDelete = await _dataAccess.GetChatGroupByIdAsync(1);
        //    int chatGroupToDeleteId = chatGroupToDelete.ChatGroupId;

        //    _context.Database.BeginTransaction();
        //    bool resultOfDelete = await _dataAccess.DeleteChatGroupAsync(chatGroupToDelete);
        //    _context.ChangeTracker.Clear();

        //    bool chatGroupExists = _dataAccess.ChatGroupexists(chatGroupToDeleteId);
        //    Assert.Multiple(() =>
        //    {
        //        Assert.True(resultOfDelete);
        //        Assert.False(chatGroupExists);
        //    });
        //    Assert.False(chatGroupExists);
        //}

        [Fact]
        public async Task AddUserToPrivateChatGroup_AddsUser()
        {
            int groupToJoinId = 4;
            string userIdToJoin = "e08b0077-3c15-477e-84bb-bf9d41196455";
            PrivateGroupMembers newPrivateGroupMember = GetNewPrivateGroupMember(groupToJoinId, userIdToJoin);

            _context.Database.BeginTransaction();
            bool resultOfAdd = await _dataAccess.AddUserToPrivateChatGroupAsync(newPrivateGroupMember);
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
                Assert.True(resultOfAdd);
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
            PrivateGroupMembers privateGroupMember = await _dataAccess.GetPrivateGroupMemberRecord(recordIdToDelete, userId);

            _context.Database.BeginTransaction();
            bool resultOfRemove = await _dataAccess.RemoveUserFromPrivateChatGroup(privateGroupMember);
            _context.ChangeTracker.Clear();

            bool recordExists = _context.PrivateGroupsMembers
                .Any(p => p.PrivateGroupMemberId == recordIdToDelete);

            Assert.Multiple(() =>
            {
                Assert.True(resultOfRemove);
                Assert.False(recordExists);
            });
        }

        //[Fact]
        //public void GetPrivateChatGroupsByUserId_ReturnsCorrectList()
        //{
        //    string userId = "e08b0077-3c15-477e-84bb-bf9d41196455";
        //    List<ChatGroups> listPrivateChatGroups = GetListPrivateChatGroups(userId);

        //    Mock<IChatGroupsDataAccess> _mockDataAccess = new();
        //    _mockDataAccess.Setup(x => x.GetViewListPrivateChatGroupsByUserId(userId))
        //        .Returns(listPrivateChatGroups);

        //    var mockedDataAccessObject = _mockDataAccess.Object;
        //    var result = mockedDataAccessObject.GetViewListPrivateChatGroupsByUserId(userId);

        //    Assert.Equal(listPrivateChatGroups, result);
        //}

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

        private PrivateGroupMembers GetNewPrivateGroupMember(int groupToJoinId, string userIdToJoin)
        {
            return new()
            {
                PrivateChatGroupId = groupToJoinId,
                UserId = userIdToJoin
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
