using ChatApplicationModels;
using Microsoft.Extensions.Configuration;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Models.Views;
using static SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PublicChatGroupsDatabaseFixture;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class PublicChatGroupsDataAccess_UnitTests : IClassFixture<PublicChatGroupsDatabaseFixture>
    {
        public PublicChatGroupsDatabaseFixture Fixture { get; }
        private readonly PublicChatGroupsDataAccess _dataAccess;
        private readonly TestChatGroupsDbContext _context;
        private readonly IConfiguration _configuration;

        public PublicChatGroupsDataAccess_UnitTests(PublicChatGroupsDatabaseFixture fixture)
        {
            Fixture = fixture;
            _context = Fixture.CreateContext();
            _configuration = new Mock<IConfiguration>().Object;
            _dataAccess = new PublicChatGroupsDataAccess(_context, _configuration);
        }

        [Fact]
        public async Task GetViewListAsync_ReturnsChatGroups()
        {
            int expectedGroupsCount = _context.PublicChatGroups
                .ToList()
                .Count;
            List<PublicChatGroupsView> viewList = _context.ChatGroupsViews
                .ToList();

            Mock<IPublicChatGroupsDataAccess> _mockChatGroupsDataAccess = new();
            _mockChatGroupsDataAccess.Setup(c => c.GetViewListAsync())
                .ReturnsAsync(viewList);

            List<PublicChatGroupsView> result = await _mockChatGroupsDataAccess.Object
                .GetViewListAsync();

            Assert.Multiple(() =>
            {
                Assert.Equal(expectedGroupsCount, result.Count);
                Assert.NotNull(result);
            });
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectChatGroup()
        {
            string expectedChatGroupName = "TestPublicGroup1";
            PublicChatGroupsView view = _context.ChatGroupsViews
                .Single(c => c.ChatGroupId == 1);
            int expectedGroupId = view.ChatGroupId;

            Mock<IPublicChatGroupsDataAccess> _mockChatGroupsDataAccess = new();
            _mockChatGroupsDataAccess.Setup(c => c.GetViewByIdAsync(1))
                .ReturnsAsync(view);
                

            PublicChatGroupsView resultChatGroup = await _mockChatGroupsDataAccess.Object
                .GetViewByIdAsync(1);

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
            string goodName = _context.PublicChatGroups
                .OrderBy(c => c.ChatGroupId)
                .First()
                .ChatGroupName;

            PublicChatGroups goodChatGroup = _dataAccess.GetByGroupName(goodName);

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
            string takenName = _context.PublicChatGroups
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
            PublicChatGroups newChatGroup = GetNewPublicChatGroup();

            _context.Database.BeginTransaction();
            bool resultOfAdd = await _dataAccess.AddAsync(newChatGroup);
            _context.ChangeTracker.Clear();

            PublicChatGroups resultChatGroup = _context.PublicChatGroups
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
            PublicChatGroups chatGroupToModify = _context.PublicChatGroups
                .Single(c => c.ChatGroupId == chatGroupId);
            chatGroupToModify.ChatGroupName = expectedModifiedGroupName;

            _context.Database.BeginTransaction();
            bool resultOfModify = await _dataAccess.ModifyAsync(chatGroupToModify);
            _context.ChangeTracker.Clear();

            PublicChatGroups modifiedChatGroup = _context.PublicChatGroups
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
            PublicChatGroups chatGroupToDelete = _context.PublicChatGroups
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

        #region PRIVATE METHODS

        private PublicChatGroups GetNewPublicChatGroup()
        {
            return new()
            {
                ChatGroupName    = "NewPublicChatGroup",
                GroupCreated     = DateTime.Now,
                GroupOwnerUserId = "93eeda54-e362-49b7-8fd0-ab516b7f8071"
            };
        }

        #endregion
    }
}
