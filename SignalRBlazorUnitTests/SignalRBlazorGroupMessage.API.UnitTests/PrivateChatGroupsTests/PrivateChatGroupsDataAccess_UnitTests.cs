using ChatApplicationModels;
using Microsoft.Extensions.Configuration;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Models.Dtos;
using SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateChatGroupsTests;
using System.Text.RegularExpressions;
using static SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateChatGroupsTests.PrivateChatGroupsDatabaseFixture;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests.PrivateChatGroups
{
    public class PrivateChatGroupsDataAccess_UnitTests : IClassFixture<PrivateChatGroupsDatabaseFixture>
    {
        public PrivateChatGroupsDatabaseFixture Fixture { get; }
        private readonly PrivateChatGroupsDataAccess _dataAccess;
        private readonly TestPrivateChatGroupDbContext _context;
        private readonly IConfiguration _configuration;

        public PrivateChatGroupsDataAccess_UnitTests(PrivateChatGroupsDatabaseFixture fixture)
        {
            Fixture = fixture;
            _context = Fixture.CreateContext();
            _configuration = new Mock<IConfiguration>().Object;
            _dataAccess = new PrivateChatGroupsDataAccess(_context, _configuration);
        }

        [Fact]
        public async Task GetDtoListByUserIdAsync_ReturnsChatGroups()
        {
            List<PrivateChatGroupsDto> dtoList = _context.PrivateChatGroupsDto.ToList();
            string userId = dtoList.First().GroupOwnerUserId;

            Mock<IPrivateChatGroupsDataAccess> _mockChatGroupsDataAccess = new();
            _mockChatGroupsDataAccess.Setup(mock => mock.GetDtoListByUserIdAsync(userId))
                .ReturnsAsync(dtoList);

            var result = await _mockChatGroupsDataAccess.Object.GetDtoListByUserIdAsync(userId);

            Assert.Multiple(() =>
            {
                Assert.Equal(dtoList.Count, result.Count);
                Assert.NotNull(result);
            });
        }

        [Fact]
        public async Task GetDtoByGroupIdAsync_ReturnsCorrectGroup()
        {
            PrivateChatGroupsDto dto = _context.PrivateChatGroupsDto.First();
            int groupId = dto.ChatGroupId;

            Mock<IPrivateChatGroupsDataAccess> _mockChatGroupsDataAccess = new();
            _mockChatGroupsDataAccess.Setup(mock => mock.GetDtoByGroupIdAsync(groupId))
                .ReturnsAsync(dto);

            var result = await _mockChatGroupsDataAccess.Object.GetDtoByGroupIdAsync(groupId);

            Assert.Multiple(() =>
            {
                Assert.Equal(dto, result);
                Assert.NotNull(result);
            });
        }

        [Fact]
        public void GetByGroupName_ReturnsCorrectGroup()
        {
            var group = _context.PrivateChatGroups.First();
            string groupName = group.ChatGroupName;

            var result = _dataAccess.GetByGroupname(groupName);

            Assert.Equal(group, result);
        }

        [Fact]
        public void GetByGroupId_ReturnsCorrectGroup()
        {
            var group = _context.PrivateChatGroups.Skip(1).First();
            int groupId = group.ChatGroupId;

            var result = _dataAccess.GetByGroupId(groupId);

            Assert.Equal(group, result);
        }

        [Fact]
        public void GroupNameTaken_ReturnsTrue()
        {
            var groupName = _context.PrivateChatGroups.First().ChatGroupName;

            bool result = _dataAccess.GroupNameTaken(groupName);

            Assert.True(result);
        }

        [Fact]
        public async Task GroupExists_ReturnsTrue()
        {
            var groupId = _context.PrivateChatGroups.Skip(2).First().ChatGroupId;

            bool result = await _dataAccess.GroupExists(groupId);

            Assert.True(result);
        }

        [Fact]
        public async Task GetPrivateGroupMemberRecord_ReturnsCorrectResult()
        {
            PrivateGroupMembers member = _context.PrivateGroupMembers.First();

            var result = await _dataAccess.GetPrivateGroupMemberRecord(member.PrivateChatGroupId, member.UserId);

            Assert.Equal(member, result);
        }

        [Fact]
        public async Task AddUserToGroupAsync_IsSuccess()
        {
            PrivateGroupMembers newMember = new() { PrivateChatGroupId = 1, UserId = Guid.NewGuid().ToString() };
            int currentCount = _context.PrivateGroupMembers.Where(c => c.PrivateChatGroupId == 1).Count();

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.AddUserToGroupAsync(newMember);
            _context.ChangeTracker.Clear();

            Assert.Multiple(() =>
            {
                Assert.True(result);
                int updatedCount = _context.PrivateGroupMembers.Where(c => c.PrivateChatGroupId == 1).Count();
                Assert.True(currentCount + 1 == updatedCount);
            });
        }

        [Fact]
        public async Task RemoveUserFromPrivateChatGroup_IsSuccess()
        {
            PrivateGroupMembers memberToDelete = _context.PrivateGroupMembers.First();

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.RemoveUserFromPrivateChatGroup(memberToDelete);
            _context.ChangeTracker.Clear();

            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.False(_context.PrivateGroupMembers
                    .Any(id => id.PrivateGroupMemberId == memberToDelete.PrivateGroupMemberId));
            });
        }

        [Fact]
        public async Task RemoveAllUsersFromGroupAsync_RemovesUsers()
        {
            int groupId = _context.PrivateGroupMembers.First().PrivateChatGroupId;

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.RemoveAllUsersFromGroupAsync(groupId);
            _context.ChangeTracker.Clear();

            int resultCount = _context.PrivateGroupMembers.Where(id => id.PrivateChatGroupId == groupId).Count();
            Assert.Equal(0, resultCount);
        }

        [Fact]
        public async Task IsUserInPrivateGroup_IsTrue()
        {
            PrivateGroupMembers member = _context.PrivateGroupMembers.First();

            bool result = await _dataAccess.IsUserInPrivateGroup(member.PrivateChatGroupId, member.UserId);

            Assert.True(result);
        }

        [Fact]
        public async Task AddAsync_IsSuccess()
        {
            int currentGroupCount = _context.PrivateChatGroups.Count();

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.AddAsync(GetNewGroup());
            _context.ChangeTracker.Clear();

            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.True(currentGroupCount + 1 == _context.PrivateChatGroups.Count());
            });
        }

        [Fact]
        public async Task ModifyAsync_IsSuccess()
        {
            var group = _context.PrivateChatGroups.First();
            string modifiedGroupName = "Modified";
            group.ChatGroupName = modifiedGroupName;

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.ModifyAsync(group);
            _context.ChangeTracker.Clear();

            string resultName = _context.PrivateChatGroups.First().ChatGroupName;
            Assert.Equal(modifiedGroupName, resultName);
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            var group = _context.PrivateChatGroups.First();
            int groupId = group.ChatGroupId;

            _context.Database.BeginTransaction();
            bool result = await _dataAccess.DeleteAsync(group);
            _context.ChangeTracker.Clear();

            Assert.False(_context.PrivateChatGroups.Any(id => id.ChatGroupId == groupId));
        }

        #region PRIVATE METHODS

        public ChatApplicationModels.PrivateChatGroups GetNewGroup()
        {
            return new()
            {
                ChatGroupName    = "NewGroup",
                GroupCreated     = DateTime.UtcNow,
                GroupOwnerUserId = Guid.NewGuid().ToString()
            };
        }

        #endregion
    }
}
