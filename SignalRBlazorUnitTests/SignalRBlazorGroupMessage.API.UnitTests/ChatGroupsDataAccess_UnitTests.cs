using ChatApplicationModels;
using Microsoft.EntityFrameworkCore;
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

            List<ChatGroupsView> result = await _mockChatGroupsDataAccess.Object
                .GetViewListPublicChatGroupsAsync();

            Assert.Multiple(() =>
            {
                Assert.Equal(expectedPublicChatGroupsCount, result.Count);
                Assert.NotNull(result);
            });
        }

        [Fact]
        public async Task GetChatGroupById_ReturnsCorrectChatGroup()
        {
            string expectedChatGroupName = "TestPublicGroup1";
            ChatGroupsView view = _context.ChatGroupsViews
                .Single(c => c.ChatGroupId == 1);
            int expectedGroupId = view.ChatGroupId;

            Mock<IChatGroupsDataAccess> _mockChatGroupsDataAccess = new();
            _mockChatGroupsDataAccess.Setup(c => c.GetChatGroupByIdAsync(1))
                .ReturnsAsync(view);
                

            ChatGroupsView resultChatGroup = await _mockChatGroupsDataAccess.Object
                .GetChatGroupByIdAsync(1);

            Assert.Multiple(() =>
            {
                Assert.Equal(expectedChatGroupName, resultChatGroup.ChatGroupName);
                Assert.Equal(expectedGroupId, resultChatGroup.ChatGroupId);
            });
        }

        [Fact]
        public void GetByGroupName_ReturnsCorrectResult()
        {
            string badName = "shouldnotreturnanything";
            string goodName = _context.ChatGroups
                .OrderBy(c => c.ChatGroupId)
                .First()
                .ChatGroupName;

            ChatGroups goodChatGroup = _dataAccess.GetByGroupName(goodName);

            Assert.Multiple(() =>
            {
                Assert.Equal(goodName, goodChatGroup.ChatGroupName);
                Assert.Throws<InvalidOperationException>(() => _dataAccess.GetByGroupName(badName));
            });
        }

        [Fact]
        public void GroupNameTaken_ReturnsCorrectResult()
        {
            string notTakenName = "ShouldReturnFalse";
            string takenName = _context.ChatGroups
                .OrderBy(c => c.ChatGroupId)
                .First()
                .ChatGroupName;

            bool shouldBeFalse = _dataAccess.GroupNameTaken(notTakenName);
            bool shouldBeTrue = _dataAccess.GroupNameTaken(takenName);

            Assert.Multiple(() =>
            {
                Assert.False(shouldBeFalse);
                Assert.True(shouldBeTrue);
            });
        }

        [Fact]
        public void GroupExists_ReturnsCorrectResult()
        {
            bool shouldBeTrue = _dataAccess.GroupExists(1);
            bool shouldBeFalse = _dataAccess.GroupExists(999);

            Assert.Multiple(() =>
            {
                Assert.True(shouldBeTrue);
                Assert.False(shouldBeFalse);
            });
        }

        [Fact]
        public async Task AddAsync_IsSuccess()
        {
            string expectedNewChatGroupName = "NewPublicChatGroup";
            ChatGroups newChatGroup = GetNewPublicChatGroup();

            _context.Database.BeginTransaction();
            bool resultOfAdd = await _dataAccess.AddAsync(newChatGroup);
            _context.ChangeTracker.Clear();

            ChatGroups resultChatGroup = _context.ChatGroups
                .OrderBy(c => c.ChatGroupId)
                .Last();

            Assert.Multiple(() =>
            {
                Assert.True(resultOfAdd);
                Assert.Equal(expectedNewChatGroupName, resultChatGroup.ChatGroupName);
            });
        }

        [Fact]
        public async Task ModifyAsync_IsSuccess()
        {
            int chatGroupId = 1;
            string expectedModifiedGroupName = "ModifiedChatGroupName";
            ChatGroups chatGroupToModify = _context.ChatGroups
                .Single(c => c.ChatGroupId == chatGroupId);
            chatGroupToModify.ChatGroupName = expectedModifiedGroupName;

            _context.Database.BeginTransaction();
            bool resultOfModify = await _dataAccess.ModifyAsync(chatGroupToModify);
            _context.ChangeTracker.Clear();

            ChatGroups modifiedChatGroup = _context.ChatGroups
                .Single(c => c.ChatGroupId == chatGroupId);

            Assert.Multiple(() =>
            {
                Assert.True(resultOfModify);
                Assert.Equal(expectedModifiedGroupName, modifiedChatGroup.ChatGroupName);
            });
        }

        [Fact]
        public async Task DeleteChatGroupAsync_IsSuccess()
        {
            int chatGroupId = 1;
            ChatGroups chatGroupToDelete = _context.ChatGroups
                .Single(c => c.ChatGroupId == chatGroupId);

            _context.Database.BeginTransaction();
            bool resultOfDelete = await _dataAccess.DeleteAsync(chatGroupToDelete);
            _context.ChangeTracker.Clear();

            bool chatGroupExists = _dataAccess.GroupExists(chatGroupId);
            Assert.Multiple(() =>
            {
                Assert.True(resultOfDelete);
                Assert.False(chatGroupExists);
            });
            Assert.False(chatGroupExists);
        }

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

        [Fact]
        public async void GetPrivateChatGroupsByUserId_ReturnsCorrectList()
        {
            Guid userId = Guid.Parse("e08b0077-3c15-477e-84bb-bf9d41196455");
            List<ChatGroupsView> listPrivateChatGroups = GetListPrivateChatGroups(userId);

            Mock<IChatGroupsDataAccess> _mockDataAccess = new();
            _mockDataAccess.Setup(x => x.GetViewListPrivateByUserIdAsync(userId))
                .ReturnsAsync(listPrivateChatGroups);

            var mockedDataAccess = _mockDataAccess.Object;
            var result = await mockedDataAccess
                .GetViewListPrivateByUserIdAsync(userId);

            Assert.Equal(listPrivateChatGroups, result);
        }

        #region PRIVATE METHODS

        private ChatGroups GetNewPublicChatGroup()
        {
            return new()
            {
                ChatGroupName    = "NewPublicChatGroup",
                GroupCreated     = DateTime.Now,
                GroupOwnerUserId = Guid.Parse("93eeda54-e362-49b7-8fd0-ab516b7f8071"),
                PrivateGroup     = false
            };
        }

        private PrivateGroupMembers GetNewPrivateGroupMember(int groupToJoinId, string userIdToJoin)
        {
            return new()
            {
                PrivateChatGroupId = groupToJoinId,
                UserId             = userIdToJoin
            };
        }

        // Mock of stored procedure sp_getPrivateChatGroupsForUser @UserId
        private List<ChatGroupsView> GetListPrivateChatGroups(Guid userId)
        {
            List<PrivateGroupMembers> listPrivateGroupMembers = _context.PrivateGroupsMembers
                .Where(p => p.UserId == userId.ToString())
                .ToList();

            List<ChatGroupsView> listPrivateChatGroups = new();
            foreach (var listItem in listPrivateGroupMembers)
            {
                ChatGroups chatGroup = _context.ChatGroups
                    .Single(c => c.ChatGroupId == listItem.PrivateChatGroupId);

                if (chatGroup != null)
                {
                    listPrivateChatGroups.Add(ChatGroupToView(chatGroup));
                }
            }

            return listPrivateChatGroups;
        }

        private ChatGroupsView ChatGroupToView(ChatGroups chatGroup)
        {
            return new()
            {
                ChatGroupId      = chatGroup.ChatGroupId,
                ChatGroupName    = chatGroup.ChatGroupName,
                GroupCreated     = chatGroup.GroupCreated,
                GroupOwnerUserId = chatGroup.GroupOwnerUserId,
                PrivateGroup     = chatGroup.PrivateGroup
            };
        }
        #endregion
    }
}
