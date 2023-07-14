using ChatApplicationModels;
using Moq;
using SignalRBlazorGroupsMessages.API.DataAccess;
using SignalRBlazorGroupsMessages.API.Helpers;
using SignalRBlazorGroupsMessages.API.Models;
using SignalRBlazorGroupsMessages.API.Services;

namespace SignalRBlazorUnitTests.SignalRBlazorGroupMessage.API.UnitTests
{
    public class PublicChatGroupsService_UnitTests
    {
        private readonly Mock<IPublicChatGroupsDataAccess> _mockDataAccess;
        private readonly Mock<IPublicMessagesService> _mockMessagesService;
        private readonly Mock<ISerilogger> _mockSerilogger;

        public PublicChatGroupsService_UnitTests()
        {
            _mockDataAccess = new Mock<IPublicChatGroupsDataAccess>();
            _mockMessagesService = new Mock<IPublicMessagesService>();
            _mockSerilogger = new Mock<ISerilogger>();
        }

        [Fact]
        public async Task GetListPublicChatGroupsAsync_ReturnsCorrectResult()
        {
            int excpectedCount = GetViewListPublicChatGroups().Count;

            _mockDataAccess.Setup(p => p.GetViewListAsync())
                .ReturnsAsync(GetViewListPublicChatGroups());

            PublicChatGroupsService _service = new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);

            var result = await _service.GetListPublicChatGroupsAsync();

            Assert.Multiple(() =>
            {
                Assert.Equal(excpectedCount, result.Data?.Count);
                Assert.True(result.Success);
                Assert.True(result.Message == "ok");
            });
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectResult()
        {
            PublicChatGroupsView view = GetViewListPublicChatGroups().First();

            _mockDataAccess.Setup(p => p.GetByIdAsync(1))
                .ReturnsAsync(GetViewListPublicChatGroups()
                    .Single(p => p.ChatGroupId == 1));
            _mockDataAccess.Setup(p => p.GroupExists(1))
                .Returns(true);

            PublicChatGroupsService _service = new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);

            var goodResult = await _service.GetByIdAsync(1);
            var badResult = await _service.GetByIdAsync(999);

            Assert.Multiple(() =>
            {
                //Assert.Equal(1, goodResult.Data?.ChatGroupId);
                Assert.True(goodResult.Success);

                Assert.False(badResult.Success);
                Assert.Null(badResult.Data);
            });
        }

        [Fact]
        public async Task AddAsync_IsSuccess()
        {
            PublicChatGroups newGroup = GetNewGroup();
            string expectedGroupName = newGroup.ChatGroupName;

            _mockDataAccess.Setup(p => p.GroupNameTaken(newGroup.ChatGroupName))
                .Returns(false);
            _mockDataAccess.Setup(p => p.AddAsync(It.IsAny<PublicChatGroups>()))
                .ReturnsAsync(true);

            PublicChatGroupsService _service = new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);

            var result = await _service.AddAsync(GetNewGroupDto(newGroup));

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Equal(expectedGroupName, result.Data?.ChatGroupName);
            });
        }

        [Fact]
        public async Task ModifyAsync_IsSuccess()
        {
            string modifiedGroupName = "Modified Group Name";
            var groupToModify = GetViewListPublicChatGroups().First();
            groupToModify.ChatGroupName = modifiedGroupName;

            _mockDataAccess.Setup(p => p.GetByIdAsync(groupToModify.ChatGroupId))
                .ReturnsAsync(groupToModify);
            _mockDataAccess.Setup(p => p.GroupNameTaken(modifiedGroupName))
                .Returns(false);
            _mockDataAccess.Setup(p => p.ModifyAsync(It.IsAny<PublicChatGroups>())) 
                .ReturnsAsync(true);

            PublicChatGroupsService _service = new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);

            var result = await _service.ModifyAsync(ViewToDto(groupToModify));

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
            });
        }

        [Fact]
        public async Task DeleteAsync_IsSuccess()
        {
            PublicChatGroupsView groupToDelete = GetViewListPublicChatGroups().First();
            PublicChatGroupsDto dtoToDelete = ViewToDto(groupToDelete);

            _mockDataAccess.Setup(p => p.GroupExists(dtoToDelete.ChatGroupId))
                .Returns(true);
            _mockDataAccess.Setup(p => p.DeleteAsync(It.IsAny<PublicChatGroups>()))
                .ReturnsAsync(true);
            _mockMessagesService.Setup(p => p.DeleteAllMessagesInGroupAsync(dtoToDelete.ChatGroupId))
                .ReturnsAsync(true);

            PublicChatGroupsService _service = new(_mockDataAccess.Object, _mockMessagesService.Object, _mockSerilogger.Object);

            var result = await _service.DeleteAsync(dtoToDelete);

            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.True(result.Message == "ok");
            });
        }

        #region PRIVATE METHODS

        private List<PublicChatGroupsView> GetViewListPublicChatGroups()
        {
            List<PublicChatGroupsView> groupList = new()
            {
                new()
                {
                    ChatGroupId      = 1,
                    ChatGroupName    = "Test Group 1",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName         = "Test Owner"
                },
                new()
                {
                    ChatGroupId      = 2,
                    ChatGroupName    = "Test Group 2",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName         = "Test Owner"
                },
                new()
                {
                    ChatGroupId      = 3,
                    ChatGroupName    = "Test Group 3",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName         = "Test Owner"
                },
                new()
                {
                    ChatGroupId      = 4,
                    ChatGroupName    = "Test Group 4",
                    GroupCreated     = DateTime.Now,
                    GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a"),
                    UserName         = "Test Owner"
                }
            };
            return groupList;
        }

        private PublicChatGroups GetNewGroup()
        {
            return new()
            {
                ChatGroupId      = 5,
                ChatGroupName    = "Test Group 5",
                GroupCreated     = DateTime.Now,
                GroupOwnerUserId = Guid.Parse("e1b9cf9a-ff86-4607-8765-9e47a305062a")
            };
        }

        private PublicChatGroupsDto GetNewGroupDto(PublicChatGroups newGroup)
        {
            return new()
            {
                ChatGroupId      = newGroup.ChatGroupId,
                ChatGroupName    = newGroup.ChatGroupName,
                GroupCreated     = newGroup.GroupCreated,
                GroupOwnerUserId = newGroup.GroupOwnerUserId,
                UserName         = "Test Owner 2"
            };
        }

        private PublicChatGroupsDto ViewToDto(PublicChatGroupsView view)
        {
            return new()
            {
                ChatGroupId      = view.ChatGroupId,
                ChatGroupName    = view.ChatGroupName,
                GroupCreated     = view.GroupCreated,
                GroupOwnerUserId = view.GroupOwnerUserId,
                UserName         = view.UserName
            };
        }

        #endregion
    }
}
